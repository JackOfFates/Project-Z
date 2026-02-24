Imports ProjectZ.Shared.Audio
Imports ProjectZ.Shared.Audio.Quantization

Namespace [Shared].Audio

    Public MustInherit Class MonitorSource

        Public MustOverride Function FFTLength() As Integer

        Public Event FftCalculated(sender As Object, e As FftEventArgs)
        Public Event MaximumCalculated(sender As Object, e As MaxSampleEventArgs)
        Public Event OnSignalPeak(signal As SignalRegister)

        Public WithEvents SignalMonitor As New SignalMonitor(Me)

        Public Sub New()

        End Sub

        Protected Friend Sub OnFftCalculated(e As FftEventArgs)
            RaiseEvent FftCalculated(Me, e)
        End Sub

        Protected Friend Sub OnMaximumCalculated(e As MaxSampleEventArgs)
            RaiseEvent MaximumCalculated(Me, e)
        End Sub

        Public MustOverride Sub BeginMonitoring()

        Public MustOverride Sub EndMonitoring()

        Private Sub SignalMonitor_OnSignalPeak(signal As SignalRegister) Handles SignalMonitor.OnSignalPeak
            RaiseEvent OnSignalPeak(signal)
        End Sub
    End Class

    Public Class PlaybackMonitor
        Inherits MonitorSource

        Public AudioEngine As PlaybackEngine

        Public Property Filename As String
            Get
                Return _Filename
            End Get
            Set(value As String)
                _Filename = value
                AudioEngine.Load(Filename)
            End Set
        End Property
        Private _Filename As String

        Public Sub New(AudioEngine As PlaybackEngine)
            Me.AudioEngine = AudioEngine
        End Sub

        Public Sub New()
            AudioEngine = New PlaybackEngine
        End Sub

        Public Overrides Sub BeginMonitoring()
            AddHandler AudioEngine.FftCalculated, AddressOf AudioEngine_FftCalculated
            AddHandler AudioEngine.MaximumCalculated, AddressOf AudioEngine_MaximumCalculated
        End Sub

        Public Overrides Sub EndMonitoring()
            If AudioEngine.State = NAudio.Wave.PlaybackState.Playing Then
                RemoveHandler AudioEngine.FftCalculated, AddressOf AudioEngine_FftCalculated
                RemoveHandler AudioEngine.MaximumCalculated, AddressOf AudioEngine_MaximumCalculated
            End If
        End Sub

        Private Sub AudioEngine_FftCalculated(sender As Object, e As FftEventArgs)
            OnFftCalculated(e)
        End Sub

        Private Sub AudioEngine_MaximumCalculated(sender As Object, e As MaxSampleEventArgs)
            OnMaximumCalculated(e)
        End Sub

        Public Overrides Function FFTLength() As Integer
            Return AudioEngine.GetFFTLength
        End Function
    End Class

    Public Class LoopbackMonitor
        Inherits MonitorSource

        Public WithEvents LoopbackCapture As LoopbackCapture

        Public Sub New(Capture As LoopbackCapture)
            Me.LoopbackCapture = Capture
        End Sub

        Public Sub New()
            LoopbackCapture = New LoopbackCapture
        End Sub

        Public Overrides Sub BeginMonitoring()
            If Not LoopbackCapture.isRecording And Not LoopbackCapture.isStopping Then
                LoopbackCapture.StartRecording()
                AddHandler LoopbackCapture.FftCalculated, AddressOf Capture_FftCalculated
                AddHandler LoopbackCapture.MaximumCalculated, AddressOf Capture_MaximumCalculated
            End If
        End Sub

        Public Overrides Sub EndMonitoring()
            If LoopbackCapture.isRecording And Not LoopbackCapture.isStopping Then
                LoopbackCapture.StopRecording()
                RemoveHandler LoopbackCapture.FftCalculated, AddressOf Capture_FftCalculated
                RemoveHandler LoopbackCapture.MaximumCalculated, AddressOf Capture_MaximumCalculated
            End If
        End Sub

        Private Sub Capture_FftCalculated(sender As Object, e As FftEventArgs)
            OnFftCalculated(e)
        End Sub

        Private Sub Capture_MaximumCalculated(sender As Object, e As MaxSampleEventArgs)
            OnMaximumCalculated(e)
        End Sub

        Public Overrides Function FFTLength() As Integer
            Return LoopbackCapture.Aggregator.fftLength
        End Function

    End Class

End Namespace