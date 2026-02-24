Imports NAudio.Dsp
Imports ProjectZ.Shared.Audio
Imports System.Threading


Namespace [Shared].Audio.Quantization

    Public Class SignalMonitor

#Region "Frequency Registers"

        Public ReadOnly Property FrequencyRegisters As New Dictionary(Of String, SignalRegister)

        Public Sub RegisterFrequencyRange(Signal As SignalRegister)
            FrequencyRegisters.Add(Signal.Name, Signal)
        End Sub

#End Region

#Region "Threading"

        Public Property Running As Boolean = False
        Private MonitorThread As Thread
        Private monitorCts As CancellationTokenSource

        Private Sub ResetMethod(token As CancellationToken)
            Do While Running AndAlso Not token.IsCancellationRequested

                If FrequencyRegisters.ContainsKey("beatcount") Then
                    'Dim BPM As Double = FrequencyRegisters("beatcount") * 16
                    'RaiseEvent OnTimeSignatureResolved(BPM)
                End If

                If token.WaitHandle.WaitOne(TimeSpan.FromMilliseconds(3750.0)) Then
                    Exit Do
                End If
            Loop
        End Sub

        Private Sub [stop]()
            Running = False
            If monitorCts IsNot Nothing Then
                Try
                    monitorCts.Cancel()
                Catch
                End Try
            End If
        End Sub

        Private Sub Start()
            If MonitorThread IsNot Nothing Then
                [stop]()
                Try
                    If MonitorThread.IsAlive Then
                        MonitorThread.Join(TimeSpan.FromSeconds(5))
                    End If
                Catch
                End Try
                MonitorThread = Nothing
                If monitorCts IsNot Nothing Then
                    monitorCts.Dispose()
                    monitorCts = Nothing
                End If
            End If
            Running = True
            monitorCts = New CancellationTokenSource()
            MonitorThread = New Thread(Sub() ResetMethod(monitorCts.Token))
            MonitorThread.Start()
        End Sub

#End Region

        Public Event OnTimeSignatureResolved(BPM As Double)

        Public Event SignalReceived(signal As SignalRegister)

        Public Event OnSignalPeak(signal As SignalRegister)

        Private WithEvents Parent As MonitorSource

        Public Sub New(Parent As MonitorSource)
            RegisterFrequencyRange(New SignalRegister("lowBass", New FrequencyRange(20, 40)))
            RegisterFrequencyRange(New SignalRegister("midBass", New FrequencyRange(40, 80)))
            RegisterFrequencyRange(New SignalRegister("upperBass", New FrequencyRange(80, 160)))
            RegisterFrequencyRange(New SignalRegister("lowerMidrange", New FrequencyRange(160, 320)))
            RegisterFrequencyRange(New SignalRegister("middleMidrange", New FrequencyRange(320, 640)))
            RegisterFrequencyRange(New SignalRegister("upperMidrange", New FrequencyRange(640, 1280)))
            RegisterFrequencyRange(New SignalRegister("lowerTreble", New FrequencyRange(1280, 2560)))
            RegisterFrequencyRange(New SignalRegister("middleTreble", New FrequencyRange(2560, 5120)))
            RegisterFrequencyRange(New SignalRegister("upperTreble", New FrequencyRange(5120, 102000)))
            RegisterFrequencyRange(New SignalRegister("topOctave", New FrequencyRange(102000, 20400)))
            Me.Parent = Parent
            Start()
        End Sub

#Region "FFT"
        Public ReadOnly Property Frequencies As Dictionary(Of String, Double)
            Get
                Return _Frequencies
            End Get
        End Property
        Private _Frequencies As New Dictionary(Of String, Double)

        Private CurrentFrequencyLevels As New Dictionary(Of String, Double)
        Private bins As Integer = 512, channels As Integer = 2, Fs As Integer = 44100 / channels

        Private Sub Parent_FftCalculated(sender As Object, e As FftEventArgs) Handles Parent.FftCalculated
            ResolveFrequencyPeaks(e)
        End Sub

        Private Sub ResolveFrequencyPeaks(e As FftEventArgs)

            Dim l As Integer = e.Result.Length - 1
            For i As Integer = 0 To l

                '------------------------
                ' Multi Stage Processing 
                '------------------------
                'Stage 1-----------------
                '------------------------

                'GET FREQUENCY FROM BIN
                Dim f As Double = BinToFreq(i)
                If Not CurrentFrequencyLevels.ContainsKey(f) Then
                    'ADD NEW FREQUENCY
                    CurrentFrequencyLevels.Add(f, 0)
                End If
                'SET FREQUENCY TO CURRENT LEVEL
                CurrentFrequencyLevels(f) = GetFrequencyLevel(e.Result(i))

                '------------------------
                'Stage 2-----------------
                '------------------------

                If i = l Then

                    'COPY CURRENT DATA TO NEW READONLY THREAD-SAFE OBJECT
                    _Frequencies = CurrentFrequencyLevels.Copy()

                End If

                '  If Frequencies.ContainsKey(f) AndAlso Frequencies(f) >= 1 Then
                Dim frs As List(Of SignalRegister) = FrequencyRegisters.Values.ToList
                For i2 As Integer = frs.Count - 1 To 0 Step -1
                    Dim Register As SignalRegister = FrequencyRegisters.Values(i2)
                    If Register.Range.isWithin(f) Then
                        'FOUND THE REGISTER WE NEED
                        If Frequencies.ContainsKey(Register.Name) Then
                            Frequencies(Register.Name) = f
                        Else
                            Frequencies.Add(Register.Name, f)
                        End If
                    End If
                Next
                ' End If
            Next

            ' Post Processing
            For i As Integer = Frequencies.Count - 1 To 0 Step -1
                ' Peaking Frequencies
                Dim name As String = Frequencies.Keys(i)
                If FrequencyRegisters.ContainsKey(name) Then
                    RaiseEvent OnSignalPeak(FrequencyRegisters(name))
                End If
            Next
        End Sub

        Public ReturnSignalFreq As New Dictionary(Of Object, Object)

        Private Function BinToFreq(index As Integer) As Double
            Return index * Fs / bins
        End Function

        Private Function GetFrequencyLevel(c As Complex) As Double
            Dim intensityDB As Double = 10 * Math.Log10(Math.Sqrt(c.X * c.X + c.Y * c.Y))
            Dim minDB As Double = -50
            If intensityDB < minDB Then
                intensityDB = minDB
            End If
            Dim percent As Double = intensityDB / minDB
            Return percent
        End Function

#End Region

        Protected Overrides Sub Finalize()
            MyBase.Finalize()
            [stop]()
        End Sub

    End Class

    Public Class SignalAnalyzer

    End Class

    Public Class SignalRegister

        Public Sub New(Name As String, FreqRange As FrequencyRange)
            Me.Name = Name
            Me.Range = FreqRange
        End Sub

        Public Property Name As String
        Public Property Range As FrequencyRange

    End Class

    Public Class FrequencyRange

        Public Property LowFrequency As Double
            Get
                Return _LowFrequency
            End Get
            Set(value As Double)
                If (value < 20) Or (value > 19999.9) Then
                    'Throw New IndexOutOfRangeException("Value must be between 20 And 19999.9")
                Else
                    _LowFrequency = value
                    _needsUpdate = True
                End If
            End Set
        End Property
        Private _LowFrequency As Double = 20
        Public Property HighFrequency As Double
            Get
                Return _HighFrequency
            End Get
            Set(value As Double)
                If (value < 20.1) Or (value > 20000.0) Then
                    'Throw New IndexOutOfRangeException("Value must be between 20.1 And 20000")
                Else
                    _HighFrequency = value
                    _needsUpdate = True
                End If
            End Set
        End Property
        Private _HighFrequency As Double = 20000.0
        Private _needsUpdate As Boolean = False

        Public Property AverageFrequency As Double
            Get
                If _needsUpdate Then _AverageFrequency = GetAverage(LowFrequency, HighFrequency)
                Return _AverageFrequency
            End Get
            Set(value As Double)
                _AverageFrequency = value
            End Set
        End Property
        Private _AverageFrequency As Double = 10000.0

        Public Sub New(LowFrequency As Double, HighFrequency As Double)
            Me.LowFrequency = LowFrequency
            Me.HighFrequency = HighFrequency
        End Sub

        Public Function GetAverage(low As Double, high As Double) As Double
            Return (low + high) / 2
        End Function


        Public Function isWithin(Frequency As Double) As Boolean
            Return Frequency >= LowFrequency AndAlso Frequency <= HighFrequency
        End Function

    End Class

End Namespace
