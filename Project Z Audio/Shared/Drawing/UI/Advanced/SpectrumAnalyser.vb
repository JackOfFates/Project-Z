Imports Microsoft.Xna.Framework
Imports NAudio.Dsp
Imports ProjectZ.Shared.Animations
Imports ProjectZ.Shared.Animations.Easing
Imports ProjectZ.Shared.Audio

Namespace [Shared].Drawing.UI.Advanced

    Public Class SpectrumAnalyser
        Inherits PolygonElement

#Region "Properties"

        Public Property Source As MonitorSource
            Get
                Return _Source
            End Get
            Set(value As MonitorSource)
                _Source = value
                RaiseEvent SourceChanged(_Source)
            End Set
        End Property
        Private WithEvents _Source As MonitorSource

        Public Property Easing As EaseFunction
            Get
                Return _Easing
            End Get
            Set(value As EaseFunction)
                _Easing = value
            End Set
        End Property
        Private _Easing As New SineEase(EaseType.EaseInOut)

        Public Property Intensity As Double = 20

        Public Property IntensityByte As Integer
            Get
                Return _IntensityByte
            End Get
            Set(value As Integer)
                _IntensityByte = Math.Max(1, Math.Min(255, value))
                ' Map 1..255 -> ~0.25..6 gain
                Intensity = 0.25R + (5.75R * (_IntensityByte - 1) / 254.0R)
            End Set
        End Property
        Private _IntensityByte As Integer = 70

        Public Property Smoothing As Double = 0.25

        Public Property FallSpeed As Double = 0.85

        Public Property MinDB As Double = -60

        Public Property MaxDB As Double = 12

        Public Property MinFrequencyHz As Double = 20

#End Region

#Region "Drawing"

        Private lastUpdateTicks As Long = 0
        Public Property TargetFps As Integer = 75

        Private xScale As Double = 1
        Private bins As Integer = 256

        Private sampleRate As Integer = 44100

        Private Function GetNyquistHz() As Double
            Return sampleRate / 2.0R
        End Function

        Private Sub CalculateXScale()
            xScale = (Size.X / bins) * 4
        End Sub

        ' reduce the number of points we plot for a less jagged line?
        Public lastVectors As Vector2()
        Private LastHalf As Integer = 0

        Private lastYFrames As New Queue(Of Double())

        Private smoothed() As Double
        Private peaks() As Double
        Private ReadOnly Results As New List(Of Vector2)(512)

        Public Sub Update(sender As Object, e As FftEventArgs) Handles _Source.FftCalculated

            Dim fps As Integer = If(TargetFps <= 0, 0, TargetFps)
            If fps > 0 Then
                Dim nowTicks As Long = Diagnostics.Stopwatch.GetTimestamp()
                Dim minDeltaTicks As Long = CLng(Diagnostics.Stopwatch.Frequency / fps)
                If lastUpdateTicks <> 0 AndAlso (nowTicks - lastUpdateTicks) < minDeltaTicks Then
                    Return
                End If
                lastUpdateTicks = nowTicks
            End If

            Dim fftResults As NAudio.Dsp.Complex() = e.Result

            ' Cache bin results
            Results.Clear()

            ' Fix poly by adding a corner point
            Results.Add(New Vector2(0, Size.Y))

            If fftResults.Length <> bins Then

                bins = fftResults.Length
                LastHalf = fftResults.Length / 4
                CalculateXScale()

                ReDim smoothed(LastHalf - 1)
                ReDim peaks(LastHalf - 1)

                lastYFrames.Clear()

            End If

            ' Try to capture sample rate if available (falls back to 44100)
            Try
                Dim srProp = _Source.GetType().GetProperty("SampleRate")
                If srProp IsNot Nothing Then
                    Dim v = srProp.GetValue(_Source)
                    If v IsNot Nothing Then sampleRate = Convert.ToInt32(v)
                End If
            Catch
            End Try


            Dim currentY(LastHalf - 1) As Double

            For i As Integer = 0 To LastHalf - 1
                ' Convert bin level to a normalized 0..Size.Y value
                Dim yPos As Double = GetYPosLog(fftResults(i))

                ' Hide DC/subsonic content (below MinFrequencyHz) to avoid left-edge peak
                Dim nyquist As Double = GetNyquistHz()
                Dim freq As Double = (i / CDbl(Math.Max(1, LastHalf - 1))) * nyquist
                If freq < MinFrequencyHz Then
                    yPos = Size.Y
                End If

                ' Exponential smoothing (reduces jitter)
                Dim a As Double = Math.Max(0, Math.Min(1, Smoothing))
                Dim ySm As Double = (smoothed(i) * a) + (yPos * (1 - a))
                smoothed(i) = ySm

                ' Optional peak hold adds latency; use smoothed signal for responsiveness
                yPos = ySm

                ' Mild compression still, but closer to linear for snappier response
                Dim norm As Double = Math.Max(0, Math.Min(1, yPos / Math.Max(1.0R, Size.Y)))
                norm = norm ^ 0.8
                yPos = norm * Size.Y

                currentY(i) = yPos
                Dim xPos As Double = CalculateXPos(i)

                Dim prevAvgY As Double = yPos
                If lastYFrames.Count > 0 Then
                    Dim sum As Double = 0
                    Dim n As Integer = 0
                    For Each f In lastYFrames
                        If f IsNot Nothing AndAlso f.Length > i Then
                            sum += f(i)
                            n += 1
                        End If
                    Next
                    If n > 0 Then prevAvgY = sum / n
                End If

                ' Blend current value toward the last-frames average using easing as the blend curve.
                ' This makes the smoothing actually depend on the last 3 frames.
                Dim blendT As Double = Math.Max(0, Math.Min(1, Smoothing))
                If Easing IsNot Nothing AndAlso Easing.easeType <> EaseType.Ignore Then
                    blendT = Easing.Ease(blendT)
                End If
                Dim smoothTargetY As Double = (prevAvgY * blendT) + (yPos * (1 - blendT))

                If lastVectors IsNot Nothing AndAlso lastVectors.Length - 1 > i AndAlso Easing IsNot Nothing AndAlso Easing.easeType <> EaseType.Ignore Then
                    Dim EaseResult As Double = Easing.Ease(Math.Max(0, Math.Min(1, smoothTargetY / Math.Max(1.0R, Size.Y))))
                    Dim MultY As Double = Math.Max(DoubleAnimation.Interpolate(EaseResult, lastVectors(i).Y, Size.Y, smoothTargetY * (IntensityByte / 255), smoothTargetY), 0)
                    Results.Add(New Vector2(xPos, MultY))
                Else
                    Results.Add(New Vector2(xPos, smoothTargetY))
                End If

            Next

            'Fix poly by adding a corner point
            Results.Add(Size)

            lastVectors = Vectors.ToArray
            AddVectorPoints(Results.ToArray)

            lastYFrames.Enqueue(currentY)
            While lastYFrames.Count > 3
                lastYFrames.Dequeue()
            End While
        End Sub

        Private Function GetYPosLog(c As NAudio.Dsp.Complex) As Double
            Const epsilon As Double = 1.0E-20

            ' Power-based dB scaling for FFT bins
            Dim power As Double = (c.X * c.X) + (c.Y * c.Y)
            If power < epsilon Then power = epsilon

            Dim db As Double = 10 * Math.Log10(power)
            If Double.IsNaN(db) OrElse Double.IsInfinity(db) Then
                db = MinDB
            End If

            Dim minDbClamped As Double = Math.Min(MinDB, MaxDB)
            Dim maxDbClamped As Double = Math.Max(MinDB, MaxDB)
            db = Math.Max(minDbClamped, Math.Min(maxDbClamped, db))

            ' Map [MinDB..MaxDB] -> [Size.Y..0]
            Dim norm As Double = (db - minDbClamped) / (maxDbClamped - minDbClamped)
            Return (1 - norm) * Size.Y
        End Function

        Private Function CalculateXPos(bin As Integer) As Double
            If LastHalf <= 1 Then Return 0

            Dim nyquist As Double = GetNyquistHz()
            Dim minF As Double = Math.Max(1.0R, MinFrequencyHz)
            Dim maxF As Double = Math.Max(minF + 1.0R, nyquist)

            ' Bin -> frequency (approx)
            Dim freq As Double = (bin / CDbl(LastHalf - 1)) * maxF
            If freq < minF Then freq = minF

            ' Log scale mapping
            Dim logMin As Double = Math.Log10(minF)
            Dim logMax As Double = Math.Log10(maxF)
            Dim logF As Double = Math.Log10(freq)
            Dim t As Double = (logF - logMin) / (logMax - logMin)
            If Double.IsNaN(t) OrElse Double.IsInfinity(t) Then t = 0
            t = Math.Max(0, Math.Min(1, t))

            Return t * Size.X
        End Function

#End Region

#Region "Events"

        Public Event SourceChanged(newSource As MonitorSource)

#End Region

        Private Sub SpectrumAnalyser_OnSizeChange(oldSize As Vector2, newSize As Vector2) Handles Me.SizeChanged
            CalculateXScale()
        End Sub

        Public Sub New(Scene As Scene)
            MyBase.New(Scene)
        End Sub

        Public Sub New(Scene As Scene, Source As MonitorSource)
            MyBase.New(Scene)
            Me.Source = Source
        End Sub

    End Class

End Namespace
