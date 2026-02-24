Imports Microsoft.Xna.Framework
Imports Microsoft.Xna.Framework.Graphics
Imports NAudio.Dsp
Imports ProjectZ.Shared.Animations
Imports ProjectZ.Shared.Animations.Easing
Imports ProjectZ.Shared.Audio

Namespace [Shared].Drawing.UI.Advanced

    Public Class SegmentedSpectrumAnalyser
        Inherits UI.Primitives.RectangleElement

#Region "Properties"

        Public RenderProperties As New PolygonRenderProperties()

#Region "Events"

        Public Event SourceChanged(newSource As MonitorSource)

#End Region

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

        Public Property Intensity As Double = 0.5

        Public Property Bands As Integer
            Get
                Return _Bands
            End Get
            Set(value As Integer)
                _Bands = Math.Max(1, value)
                RebuildBands()
            End Set
        End Property
        Private _Bands As Integer = 4

        Public Property FillColor As Color
            Get
                Return _FillColor
            End Get
            Set(value As Color)
                _FillColor = value
                Try
                    ApplyBandColors()
                Catch
                End Try
            End Set
        End Property
        Private _FillColor As Color = Color.LimeGreen

#End Region

#Region "Drawing"

        Private xScale As Double = 1
        Private bins As Integer = 256

        Private Sub CalculateXScale()
            xScale = (Size.X / bins) * 4
        End Sub

        ' reduce the number of points we plot for a less jagged line?
        Private LastHalf As Integer = 0
        Private lastVectorsPerBand As New List(Of Vector2())
        Private PolyBands As New List(Of PolygonElement)

        Private ReadOnly sc As Scene
        Public Sub Update(sender As Object, e As FftEventArgs) Handles _Source.FftCalculated

            Dim fftResults As NAudio.Dsp.Complex() = e.Result

            If PolyBands.Count = 0 Then
                RebuildBands()
            End If

            If fftResults.Length <> bins Then

                bins = fftResults.Length
                LastHalf = fftResults.Length / 4
                CalculateXScale()

            End If

            Dim bandCount As Integer = Math.Max(1, PolyBands.Count)
            Dim binsPerBand As Integer = Math.Max(1, CInt(Math.Ceiling(LastHalf / CDbl(bandCount))))
            Dim bandWidth As Single = If(bandCount = 0, Size.X, CSng(Size.X / bandCount))

            Dim anyNonZero As Boolean = False

            For band As Integer = 0 To bandCount - 1
                Dim poly = PolyBands(band)

                ' StackPanel layout: each band is a fixed-width child placed horizontally
                poly.Position = New Vector2(Position.X + (band * bandWidth), Position.Y)
                poly.Size = New Vector2(bandWidth, Size.Y)

                Dim startBin As Integer = band * binsPerBand
                Dim endBin As Integer = Math.Min(LastHalf, startBin + binsPerBand)
                If startBin >= endBin Then
                    poly.ClearVectorPoints()
                    Continue For
                End If

                Dim localLast() As Vector2 = Nothing
                If band < lastVectorsPerBand.Count Then localLast = lastVectorsPerBand(band)

                Dim localBins As Integer = endBin - startBin
                Dim localXScale As Single
                If localBins <= 0 Then
                    localXScale = 1.0F
                Else
                    localXScale = (bandWidth / localBins) * 4.0F
                End If
                If Single.IsNaN(localXScale) OrElse Single.IsInfinity(localXScale) OrElse localXScale <= 0 Then
                    localXScale = 1.0F
                End If

                Dim results As New List(Of Vector2)(localBins + 2)
                results.Add(New Vector2(0.0F, Size.Y))

                For i As Integer = 0 To localBins - 1
                    Dim globalBin As Integer = startBin + i
                    Dim yPos As Single = CSng(GetYPosLog(fftResults(globalBin)))
                    If Not Single.IsNaN(yPos) AndAlso Not Single.IsInfinity(yPos) AndAlso yPos > 0 Then anyNonZero = True

                    Dim xPos As Single = i * localXScale

                    If Easing IsNot Nothing AndAlso Easing.easeType <> EaseType.Ignore AndAlso localLast IsNot Nothing AndAlso localLast.Length - 1 > i Then
                        Dim EaseResult As Double = Easing.Ease(yPos)
                        Dim smoothedY As Double = DoubleAnimation.Interpolate(EaseResult, localLast(i).Y, Size.Y, yPos * Intensity, yPos)
                        If Double.IsNaN(smoothedY) OrElse Double.IsInfinity(smoothedY) Then smoothedY = 0
                        If smoothedY < 0 Then smoothedY = 0
                        If smoothedY > Size.Y Then smoothedY = Size.Y
                        results.Add(New Vector2(xPos, CSng(smoothedY)))
                    Else
                        If yPos < 0 Then yPos = 0
                        If yPos > Size.Y Then yPos = Size.Y
                        results.Add(New Vector2(xPos, yPos))
                    End If
                Next

                results.Add(New Vector2(bandWidth, Size.Y))

                If band >= lastVectorsPerBand.Count Then
                    lastVectorsPerBand.Add(poly.Vectors.ToArray())
                Else
                    lastVectorsPerBand(band) = poly.Vectors.ToArray()
                End If

                ' Convert to absolute-space points for PolygonElement
                Dim absolute(results.Count - 1) As Vector2
                For j As Integer = 0 To results.Count - 1
                    absolute(j) = results(j) + poly.Position
                Next

                poly.AddVectorPoints(absolute)
            Next

            If Not anyNonZero Then
                For Each p In PolyBands
                    p.isVisible = False
                Next
                Return
            End If

            For Each p In PolyBands
                If Not p.isVisible Then p.isVisible = True
            Next
        End Sub

        Private Function GetYPosLog(c As NAudio.Dsp.Complex) As Double
            ' Use power (magnitude^2) in dB. FFT magnitudes are often very small,
            ' and 20*log10(mag) can collapse to minDB too easily.
            Const epsilon As Double = 1.0E-20
            Const minDB As Double = -80

            Dim power As Double = (c.X * c.X) + (c.Y * c.Y)
            If power < epsilon Then power = epsilon

            Dim intensityDB As Double = 10 * Math.Log10(power)
            If Double.IsNaN(intensityDB) OrElse Double.IsInfinity(intensityDB) Then
                intensityDB = minDB
            End If

            intensityDB = Math.Max(minDB, Math.Min(0, intensityDB))

            ' Map [minDB..0] -> [Size.Y..0] (silence at bottom, louder goes up)
            Dim norm As Double = (intensityDB - minDB) / (0 - minDB)
            Return (1 - norm) * Size.Y
        End Function

        Private Function CalculateXPos(bin As Integer) As Double
            If bin = 0 Then Return 0
            Return (bin * xScale)
        End Function

#End Region

        Public Sub New(Scene As Scene)
            MyBase.New(Scene)
            sc = Scene
            BackgroundColor = Color.Transparent
            RebuildBands()
        End Sub

        Public Sub New(Scene As Scene, Source As MonitorSource)
            MyBase.New(Scene)
            sc = Scene
            BackgroundColor = Color.Transparent
            RebuildBands()
            Me.Source = Source
        End Sub

        Private Sub RebuildBands()
            If spriteBatch Is Nothing Then Return

            PolyBands.Clear()
            lastVectorsPerBand.Clear()

            For i As Integer = Children.Count - 1 To 0 Step -1
                Children.Remove(Children(i))
            Next

            For i As Integer = 0 To _Bands - 1
                Dim p As New PolygonElement(sc, spriteBatch.SpriteBatch) With {
                    .HorizontalAlign = HorizontalAlignment.Left,
                    .VerticalAlign = VerticalAlignment.Bottom,
                    .isMouseBypassEnabled = True
                }

                PolyBands.Add(p)
                Children.Add(p)
            Next

            AddHandler RenderProperties.OnWireFrameColorChanged, Sub(c)
                                                                     For Each p In PolyBands
                                                                         p.RenderProperties.WireFrameColor = c
                                                                     Next
                                                                 End Sub

            ApplyRenderProperties()
            Try
                ApplyBandColors()
            Catch
            End Try
        End Sub

        Private Sub ApplyRenderProperties()
            For Each p In PolyBands
                p.RenderProperties.HighQuality = RenderProperties.HighQuality
                p.RenderProperties.MinAngle = RenderProperties.MinAngle
                p.RenderProperties.WireFrame = RenderProperties.WireFrame
                p.RenderProperties.FillPolygon = RenderProperties.FillPolygon
                p.RenderProperties.WireFrameColor = RenderProperties.WireFrameColor
            Next
        End Sub

        Private Sub ApplyBandColors()
            If PolyBands.Count = 0 Then Return

            Dim Clamp01 As Func(Of Double, Double) = Function(v As Double)
                                                         If v < 0.0R Then Return 0.0R
                                                         If v > 1.0R Then Return 1.0R
                                                         Return v
                                                     End Function

            Dim Clamp255 As Func(Of Integer, Integer) = Function(v As Integer)
                                                            If v < 0 Then Return 0
                                                            If v > 255 Then Return 255
                                                            Return v
                                                        End Function

            For i As Integer = 0 To PolyBands.Count - 1
                Dim a As Integer = _FillColor.A

                Dim col As Color
                If PolyBands.Count <= 1 Then
                    col = New Color(255, 0, 0, Clamp255(a))
                ElseIf PolyBands.Count = 2 Then
                    col = If(i = 0, New Color(255, 0, 0, Clamp255(a)), New Color(0, 0, 255, Clamp255(a)))
                ElseIf PolyBands.Count = 3 Then
                    If i = 0 Then
                        col = New Color(255, 0, 0, Clamp255(a))
                    ElseIf i = 1 Then
                        col = New Color(0, 255, 0, Clamp255(a))
                    Else
                        col = New Color(0, 0, 255, Clamp255(a))
                    End If
                Else
                    Dim denom As Integer = PolyBands.Count - 1
                    Dim t As Double = If(denom <= 0, 0.0R, i / CDbl(denom))
                    t = Clamp01(t)

                    Dim c0 As Color = Color.Red
                    Dim c1 As Color = Color.Green
                    Dim c2 As Color = Color.Blue

                    If t <= 0.5R Then
                        Dim u As Double = Clamp01(t / 0.5R)
                        Dim r As Integer = CInt(c0.R + (c1.R - c0.R) * u)
                        Dim g As Integer = CInt(c0.G + (c1.G - c0.G) * u)
                        Dim b As Integer = CInt(c0.B + (c1.B - c0.B) * u)
                        col = New Color(Clamp255(r), Clamp255(g), Clamp255(b), Clamp255(a))
                    Else
                        Dim u As Double = Clamp01((t - 0.5R) / 0.5R)
                        Dim r As Integer = CInt(c1.R + (c2.R - c1.R) * u)
                        Dim g As Integer = CInt(c1.G + (c2.G - c1.G) * u)
                        Dim b As Integer = CInt(c1.B + (c2.B - c1.B) * u)
                        col = New Color(Clamp255(r), Clamp255(g), Clamp255(b), Clamp255(a))
                    End If
                End If

                PolyBands(i).FillColor = col
                PolyBands(i).isVisible = True
            Next
        End Sub

    End Class

End Namespace
