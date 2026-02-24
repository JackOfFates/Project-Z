
Imports System.Collections.Generic
Imports System.Text
Imports NAudio.Utils
Imports NAudio.Wave


''' <summary>
''' Converts a sample provider to 16 bit PCM, optionally clipping and adjusting volume along the way
''' </summary>
Public Class SampleToWaveProvider16
    Implements IWaveProvider

    Private sourceProvider As ISampleProvider
    Private ReadOnly m_waveFormat As WaveFormat
    Private m_volume As Single
    Private sourceBuffer As Single()

    ''' <summary>
    ''' Creates a new SampleToWaveProvider16
    ''' </summary>
    ''' <param name="sourceProvider">the source provider</param>
    Public Sub New(sourceProvider As ISampleProvider)
        If sourceProvider.WaveFormat.Encoding <> WaveFormatEncoding.IeeeFloat Then
            Throw New ArgumentException("Input source provider must be IEEE float", "sourceProvider")
        End If
        If sourceProvider.WaveFormat.BitsPerSample <> 32 Then
            Throw New ArgumentException("Input source provider must be 32 bit", "sourceProvider")
        End If

        m_waveFormat = New WaveFormat(sourceProvider.WaveFormat.SampleRate, 16, sourceProvider.WaveFormat.Channels)

        Me.sourceProvider = sourceProvider
        Me.m_volume = 1.0F
    End Sub

    ''' <summary>
    ''' Reads bytes from this wave stream
    ''' </summary>
    ''' <param name="destBuffer">The destination buffer</param>
    ''' <param name="offset">Offset into the destination buffer</param>
    ''' <param name="numBytes">Number of bytes read</param>
    ''' <returns>Number of bytes read.</returns>
    Public Function Read(destBuffer As Byte(), offset As Integer, numBytes As Integer) As Integer
        Dim samplesRequired As Integer = numBytes / 2
        Me.sourceBuffer = BufferHelpers.Ensure(sourceBuffer, samplesRequired)
        Dim sourceSamples As Integer = sourceProvider.Read(sourceBuffer, 0, samplesRequired)
        Dim destWaveBuffer As New WaveBuffer(destBuffer)

        Dim destOffset As Integer = offset / 2
        For sample As Integer = 0 To sourceSamples - 1
            ' adjust volume
            Dim sample32 As Single = sourceBuffer(sample) * m_volume
            ' clip
            If sample32 > 1.0F Then
                sample32 = 1.0F
            End If
            If sample32 < -1.0F Then
                sample32 = -1.0F
            End If
            destWaveBuffer.ShortBuffer(System.Math.Max(System.Threading.Interlocked.Increment(destOffset), destOffset - 1)) = CShort(sample32 * 32767)
        Next

        Return sourceSamples * 2
    End Function

    Private Function IWaveProvider_Read(buffer() As Byte, offset As Integer, count As Integer) As Integer Implements IWaveProvider.Read
        Throw New NotImplementedException()
    End Function

    ''' <summary>
    ''' <see cref="IWaveProvider.WaveFormat"/>
    ''' </summary>
    Public ReadOnly Property WaveFormat() As WaveFormat Implements IWaveProvider.WaveFormat
        Get
            Return m_waveFormat
        End Get
    End Property

    ''' <summary>
    ''' Volume of this channel. 1.0 = full scale
    ''' </summary>
    Public Property Volume() As Single
        Get
            Return m_volume
        End Get
        Set
            m_volume = Value
        End Set
    End Property

End Class
