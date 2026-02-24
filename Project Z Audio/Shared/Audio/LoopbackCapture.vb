Imports NAudio.CoreAudioApi
Imports NAudio.Wave

Namespace [Shared].Audio

    Public Class LoopbackCapture

#Region "Events"

        Public Event FftCalculated(sender As Object, e As FftEventArgs)
        Public Event MaximumCalculated(sender As Object, e As MaxSampleEventArgs)

        Protected Friend Sub OnFftCalculated(sender As Object, e As FftEventArgs)
            RaiseEvent FftCalculated(sender, e)
        End Sub

        Protected Friend Sub OnMaximumCalculated(sender As Object, e As MaxSampleEventArgs)
            RaiseEvent MaximumCalculated(sender, e)
        End Sub

        Public Event StoppedRecording()
        Public Event CancelRecording()
        Public Event StartedRecording()

#End Region

#Region "Properties"

        Public Property isRecording As Boolean = False
        Public Property isStopping As Boolean = False

#End Region

        Dim Writer As WaveFileWriter
        Dim WithEvents Wasapi As WasapiLoopbackCapture
        Public WithEvents Aggregator As SampleAggregator
        Private LastBitsPerSample As Integer = 0
        Dim mmdev As MMDeviceEnumerator
        Dim audio As MMDevice
        Dim disposed As Boolean = False

        Private Sub Wasapi_DataAvailable(sender As Object, e As WaveInEventArgs) Handles Wasapi.DataAvailable
            Dim buffer As Byte() = e.Buffer
            Dim bytesRecorded As Integer = e.BytesRecorded

            Dim bufferIncrement As Integer = CInt(Wasapi.WaveFormat.BlockAlign / Wasapi.WaveFormat.Channels)
            Dim bitsPerSample As Integer = Wasapi.WaveFormat.BitsPerSample

            Dim index As Integer = 0
            While index < bytesRecorded
                Dim sample32 As Single = 0

                If bitsPerSample <= 16 Then
                    ' Presume 16-bit PCM WAV
                    Dim sample16 As Short = CShort((buffer(index + 1) << 8) Or buffer(index))
                    sample32 = sample16 / 32768.0F
                ElseIf bitsPerSample <= 32 Then
                    ' Presume 32-bit IEEE Float WAV
                    sample32 = BitConverter.ToSingle(buffer, index)
                Else
                    Throw New Exception(bitsPerSample + " Bits Per Sample Is Not Supported!")
                End If

                Aggregator.Add(Math.Min(Math.Max(sample32, 0), 1))
                index += bufferIncrement
            End While
        End Sub

        Public Sub StartRecording()
            If Not isRecording Then
                mmdev = New MMDeviceEnumerator
                audio = mmdev.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia)
                Wasapi = New WasapiLoopbackCapture(audio)

                Aggregator = New SampleAggregator With {.PerformFFT = True}
                AddHandler Aggregator.FftCalculated, AddressOf OnFftCalculated
                AddHandler Aggregator.MaximumCalculated, AddressOf OnMaximumCalculated
                Wasapi.StartRecording()
                RaiseEvent StartedRecording()
                isRecording = True
            End If
        End Sub

        Public Sub StartRecording(Filename As String)
            If Not isRecording Then
                mmdev = New MMDeviceEnumerator
                audio = mmdev.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia)
                Wasapi = New WasapiLoopbackCapture(audio)
                Writer = New WaveFileWriter(Filename, audio.AudioClient.MixFormat)
                Aggregator = New SampleAggregator With {.PerformFFT = True}
                AddHandler Aggregator.FftCalculated, AddressOf OnFftCalculated
                AddHandler Aggregator.MaximumCalculated, AddressOf OnMaximumCalculated
                Wasapi.StartRecording()
                RaiseEvent StartedRecording()
                isRecording = True
            End If
        End Sub

        Public Sub StopRecording()
            If isRecording AndAlso Not isStopping Then
                isStopping = True
                Wasapi.StopRecording()
            End If
        End Sub

        Public Sub Dispose()
            disposed = True
            If Wasapi IsNot Nothing Then
                Wasapi.Dispose()
                Wasapi = Nothing
            End If
            If Aggregator IsNot Nothing Then
                Aggregator = Nothing
            End If
            If audio IsNot Nothing Then audio = Nothing
            isRecording = False
        End Sub

        Private Sub Wasapi_RecordingStopped(sender As Object, e As StoppedEventArgs) Handles Wasapi.RecordingStopped
            If Wasapi IsNot Nothing Then
                Wasapi.Dispose()
                Wasapi = Nothing
            End If
            If Aggregator IsNot Nothing Then
                Aggregator = Nothing
            End If
            isRecording = False
            isStopping = False
            If Not disposed Then
                RaiseEvent StoppedRecording()
            Else
                RaiseEvent CancelRecording()
            End If
        End Sub

        Private Sub _sampleAggregator_FftCalculated(sender As Object, e As FftEventArgs) Handles Aggregator.FftCalculated
            RaiseEvent FftCalculated(sender, e)
        End Sub

        Private Sub _sampleAggregator_MaximumCalculated(sender As Object, e As MaxSampleEventArgs) Handles Aggregator.MaximumCalculated
            RaiseEvent MaximumCalculated(sender, e)
        End Sub
    End Class


End Namespace