Imports System.Collections.Generic
Imports System.Text
Imports System.Diagnostics
Imports NAudio.Dsp
Imports NAudio.Wave
Imports Microsoft.Xna.Framework

Namespace [Shared].Audio

    Public Class SampleAggregator
        Implements ISampleProvider

        Public Event MaximumCalculated As EventHandler(Of MaxSampleEventArgs)
        Private maxValue As Single
        Private minValue As Single
        Public Property NotificationCount() As Double
            Get
                Return m_NotificationCount
            End Get
            Set
                m_NotificationCount = Value
            End Set
        End Property
        Private m_NotificationCount As Double = 1

        ' FFT
        Public Event FftCalculated As EventHandler(Of FftEventArgs)
        Public Property PerformFFT() As Boolean
            Get
                Return m_PerformFFT
            End Get
            Set
                m_PerformFFT = Value
            End Set
        End Property
        Private m_PerformFFT As Boolean
        Private fftBuffer As NAudio.Dsp.Complex()
        Private fftArgs As FftEventArgs
        Private fftPos As Integer
        Private m As Integer
        Public Property fftLength As Integer
            Get
                Return _fftLength
            End Get
            Set(value As Integer)
                _fftLength = value
                If Not IsPowerOfTwo(_fftLength) Then
                    Throw New ArgumentException("FFT Length must be a power of two")
                End If
                Me.m = CInt(Math.Log(_fftLength, 2.0))
                Me.fftBuffer = New NAudio.Dsp.Complex(_fftLength - 1) {}
                Me.fftArgs = New FftEventArgs(fftBuffer)
            End Set
        End Property
        Private _fftLength As Integer = 256

        Private ReadOnly source As ISampleProvider

        Public ReadOnly Property channels As Integer
            Get
                Return _channels
            End Get
        End Property
        Private _channels As Integer = 2

        Public Sub New(Optional channels As Integer = 2, Optional fftLength As Integer = 512)
            Me._channels = channels
            Me.fftLength = fftLength
            Me.source = Nothing
        End Sub

        Public Sub New(source As ISampleProvider, Optional fftLength As Integer = 512)
            _channels = source.WaveFormat.Channels
            If Not IsPowerOfTwo(fftLength) Then
                Throw New ArgumentException("FFT Length must be a power of two")
            End If
            Me.m = CInt(Math.Log(fftLength, 2.0))
            Me.fftLength = fftLength
            Me.fftBuffer = New NAudio.Dsp.Complex(fftLength - 1) {}
            Me.fftArgs = New FftEventArgs(fftBuffer)
            Me.source = source
        End Sub

        Private Function IsPowerOfTwo(x As Integer) As Boolean
            Return (x And (x - 1)) = 0
        End Function

        Public Sub Reset()

        End Sub
        Private currentTime As Long = Stopwatch.GetTimestamp
        Private lastTime As New TimeSpan(Stopwatch.GetTimestamp)
        Public Sub Add(value As Single)

            '   Dim currentTime As New TimeSpan(Stopwatch.GetTimestamp)

            ' If currentTime.TotalMilliseconds >= lastTime.TotalMilliseconds + NotificationCount Then
            '  lastTime = currentTime

            If PerformFFT Then
                    fftBuffer(fftPos).X = value
                    fftBuffer(fftPos).Y = 0
                    fftPos += 1
                    If fftPos >= fftBuffer.Length Then
                        fftPos = 0
                        ' 1024 = 2^10
                        FastFourierTransform.FFT(True, m, fftBuffer)
                        RaiseEvent FftCalculated(Me, fftArgs)
                    End If
                End If

                maxValue = Math.Max(maxValue, value)
                minValue = Math.Min(minValue, value)
                RaiseEvent MaximumCalculated(Me, New MaxSampleEventArgs(minValue, maxValue))
                maxValue = InlineAssignHelper(minValue, 0)
            ' End If
        End Sub

        Public ReadOnly Property WaveFormat() As WaveFormat Implements ISampleProvider.WaveFormat
            Get
                Try
                    Return source.WaveFormat
                Catch ex As Exception
                    Return Nothing
                End Try
            End Get
        End Property

        Public Function Read(buffer As Single(), offset As Integer, count As Integer) As Integer Implements ISampleProvider.Read
            Dim samplesRead = source.Read(buffer, offset, count)

            Dim n As Integer = 0
            While n < samplesRead
                Add(buffer(n + offset))
                n += channels
            End While
            Return samplesRead
        End Function
        Private Shared Function InlineAssignHelper(Of T)(ByRef target As T, value As T) As T
            target = value
            Return value
        End Function

    End Class

    Public Class MaxSampleEventArgs
        Inherits EventArgs
        <DebuggerStepThrough>
        Public Sub New(minValue As Single, maxValue As Single)
            Me.MaxSample = maxValue
            Me.MinSample = minValue
        End Sub
        Public Property MaxSample() As Single
            Get
                Return m_MaxSample
            End Get
            Private Set
                m_MaxSample = Value
            End Set
        End Property
        Private m_MaxSample As Single
        Public Property MinSample() As Single
            Get
                Return m_MinSample
            End Get
            Private Set
                m_MinSample = Value
            End Set
        End Property
        Private m_MinSample As Single
    End Class

    Public Class FftEventArgs
        Inherits EventArgs
        <DebuggerStepThrough>
        Public Sub New(result As NAudio.Dsp.Complex())
            Me.Result = result
        End Sub
        Public Property Result() As NAudio.Dsp.Complex()
            Get
                Return m_Result
            End Get
            Private Set
                m_Result = Value
            End Set
        End Property
        Private m_Result As NAudio.Dsp.Complex()
    End Class

End Namespace