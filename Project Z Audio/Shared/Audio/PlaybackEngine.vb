Imports NAudio.Dsp
Imports NAudio.Wave

Namespace [Shared].Audio

    Public Class PlaybackEngine
        Implements IDisposable, IFFT

        Private Provider As IWaveProvider
        Private playbackDevice As IWavePlayer
        Private fileStream As WaveStream
        Private _lastFftResult As Complex()

#Region "Properties"

        Public ReadOnly Property UpdateDivisor As Integer
            Get
                Return _UpdateDivisor
            End Get
        End Property
        Private _UpdateDivisor As Integer = 1024

        Public Property Latency As Integer
            Get
                Return _Latency
            End Get
            Set(value As Integer)
                _Latency = value
                If playbackDevice IsNot Nothing Then
                    WaveOutInstance.DesiredLatency = _Latency
                End If
            End Set
        End Property
        Private _Latency As Integer = 5

        Public ReadOnly Property State As PlaybackState
            Get
                Return If(playbackDevice Is Nothing, Nothing, playbackDevice.PlaybackState)
            End Get
        End Property

        Public Property Volume As Single
            Get
                Return _Volume
            End Get
            Set(value As Single)
                _Volume = value
                If playbackDevice IsNot Nothing Then
                    WaveOutInstance.Volume = _Volume
                End If
            End Set
        End Property
        Private _Volume As Single = 0.8

        Public Property CurrentTime As TimeSpan
            Get
                Return If(fileStream Is Nothing, TimeSpan.Zero, fileStream.CurrentTime)
            End Get
            Set(value As TimeSpan)
                _CurrentTime = value
                If fileStream IsNot Nothing Then
                    fileStream.CurrentTime = _CurrentTime
                End If
            End Set
        End Property
        Private _CurrentTime As TimeSpan = TimeSpan.Zero

        Public ReadOnly Property TotalTime As TimeSpan
            Get
                Return If(fileStream Is Nothing, TimeSpan.Zero, fileStream.TotalTime)
            End Get
        End Property

        Private Function WaveOutInstance() As WaveOut
            Return If(playbackDevice Is Nothing, Nothing, CType(playbackDevice, WaveOut))
        End Function

#End Region

#Region "Events"

        Public Event FftCalculated(sender As Object, e As FftEventArgs)
        Public Event MaximumCalculated(sender As Object, e As MaxSampleEventArgs)

        Protected Friend Sub OnFftCalculated(e As FftEventArgs)
            RaiseEvent FftCalculated(Me, e)
        End Sub

        Protected Friend Sub OnMaximumCalculated(e As MaxSampleEventArgs)
            RaiseEvent MaximumCalculated(Me, e)
        End Sub

#End Region

        Public Overloads Sub Load(fileName As String)
            Load(fileName, _UpdateDivisor)
        End Sub

        Public Overloads Sub Load(fileName As String, UpdateDivisor As Integer)
            _UpdateDivisor = UpdateDivisor
            [Stop]()
            CloseFile()
            EnsureDeviceCreated()
            OpenFile(fileName)
        End Sub

        Private Sub CloseFile()
            If fileStream IsNot Nothing Then
                fileStream.Dispose()
                fileStream = Nothing
            End If
        End Sub

        Private Sub OpenFile(fileName As String)
            Try
                Dim inputStream As New AudioFileReader(fileName)
                fileStream = inputStream
                Dim aggregator As New SampleAggregator(inputStream)
                aggregator.PerformFFT = True

                AddHandler aggregator.FftCalculated,
                    Sub(s, a)
                        _lastFftResult = a.Result
                        OnFftCalculated(a)
                    End Sub
                AddHandler aggregator.MaximumCalculated, Sub(s, a) OnMaximumCalculated(a)

                ' Disable extention to avoid problems with MONO
                'playbackDevice.Init(aggregator, True)
                Provider = DirectCast(New SampleToWaveProvider16(aggregator), IWaveProvider)
                playbackDevice.Init(Provider)

            Catch e As Exception
                Throw New Exception("Problem reading audio file.", e)
                CloseFile()
            End Try
        End Sub

        Private Sub EnsureDeviceCreated()
            If playbackDevice Is Nothing Then
                CreateDevice()
            End If
        End Sub

        Private Sub CreateDevice()
            playbackDevice = New WaveOut() With {.DesiredLatency = Math.Max(50, Latency), .NumberOfBuffers = 2, .Volume = Volume}
        End Sub

        Public Sub Play()
            If playbackDevice IsNot Nothing AndAlso fileStream IsNot Nothing AndAlso playbackDevice.PlaybackState <> PlaybackState.Playing Then
                playbackDevice.Play()
            End If
        End Sub

        Public Function getComplexArray() As Complex()
            Dim r = _lastFftResult
            If r Is Nothing Then
                Return Nothing
            End If

            ' return a copy to prevent callers from mutating the internal buffer
            Dim copy(r.Length - 1) As Complex
            Array.Copy(r, copy, r.Length)
            Return copy
        End Function

        Public Sub Pause()
            If playbackDevice IsNot Nothing Then
                playbackDevice.Pause()
            End If
        End Sub

        Public Sub [Stop]()
            If playbackDevice IsNot Nothing Then
                playbackDevice.[Stop]()
            End If
            If fileStream IsNot Nothing Then
                fileStream.Position = 0
            End If
        End Sub

        Public Sub Dispose() Implements IDisposable.Dispose
            [Stop]()
            CloseFile()
            If playbackDevice IsNot Nothing Then
                playbackDevice.Dispose()
                playbackDevice = Nothing
            End If
        End Sub

        Public Function GetFFTLength() As Object Implements IFFT.GetFFTLength
            Return _UpdateDivisor
        End Function
    End Class

End Namespace


