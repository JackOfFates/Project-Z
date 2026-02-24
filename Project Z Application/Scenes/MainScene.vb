#Region "Using Statements"
Imports System.Collections.Generic
Imports System.Diagnostics
Imports Microsoft.Xna.Framework
Imports Microsoft.Xna.Framework.Content
Imports Microsoft.Xna.Framework.GamerServices
Imports Microsoft.Xna.Framework.Graphics
Imports Microsoft.Xna.Framework.Input
Imports Microsoft.Xna.Framework.Storage
Imports ProjectZ.Shared.Animations
Imports ProjectZ.Shared.Audio
Imports ProjectZ.Shared.Audio.Quantization
Imports ProjectZ.Shared.Content
Imports ProjectZ.Shared.Drawing
Imports ProjectZ.Shared.Drawing.UI
Imports ProjectZ.Shared.Drawing.UI.Advanced
Imports ProjectZ.Shared.Drawing.UI.Input

#End Region

''' <summary>
''' Main Application Scene
''' </summary>
Public Class MainScene
    Inherits Scene

    Dim Username As New UI.Input.Textbox(Me)
    Dim Button As New UI.Input.Button(Me)

    Private sm As SceneManager
    Dim ts As TestScene
    ''' <summary>
    ''' Initialization of the Scene.
    ''' </summary>
    ''' <remarks>Elements can be added here.</remarks>
    Public Sub New(ByRef SceneManager As SceneManager)
        MyBase.New(SceneManager)
        sm = SceneManager

        With Username
            .Text = "Enter a Username"
            .Size = New Vector2(200, 24)
            .Position = New Vector2(CSng(Me.sender.GraphicsDevice.ScissorRectangle.Width / 2 - Username.Size.X / 2),
                                    CSng(Me.sender.GraphicsDevice.ScissorRectangle.Height / 2 - Username.Size.Y / 2))
            .BackgroundColor = New Color(235, 235, 245, 33)
            .ForegroundColor = New Color(40, 40, 40)
        End With
        AddHandler Username.SizeChanged, Sub() Username.Position = New Vector2(CSng(Me.sender.GraphicsDevice.ScissorRectangle.Width / 2 - Username.Size.X / 2),
                                                                                   CSng(Me.sender.GraphicsDevice.ScissorRectangle.Height / 2 - Username.Size.Y / 2))
        AddHandler Username.Selected, Sub() If Username.Text = "Enter a Username" Then Username.Text = ""
        AddHandler Username.MouseLeftClick, Sub() If Username.Text = "Enter a Username" Then Username.Text = ""
        AddHandler Button.MouseLeftClick, AddressOf EnterScene
        AddHandler Username.OnKeyDown, Sub(Key As Keys, KeyboardState As KeyboardState) If Key = Keys.Enter Then EnterScene(New Point(CInt(Button.Size.X / 2), CInt(Button.Size.Y / 2)))

        With Button
            ' Set the button's Text
            .Text = "Enter Test Scene"
            ' Set the button's Position
            .Position = New Vector2(Username.Position.X, Username.Position.Y + Username.Size.Y)
            ' Allow the button to be moved by the user
            .isMovable = False
            ' Enable the button's premade animations
            .isAnimated = True
            ' Autosize the button to its text
            .AutoSize = ButtonAutoSize.None
            .Size = New Vector2(200, 24)
        End With

        ' Finally, add it to the Scene.
        'AddElement(Username)
        'AddElement(Button)


        Dim startButton As New Button(Me)
        AddHandler startButton.MouseLeftClick, Sub()
                                                   Try
                                                       ts = New TestScene(sm)
                                                       If sm.GetScene("TestScene") Is Nothing Then
                                                           sm.AddScene("TestScene", ts)
                                                       End If
                                                   Catch ex As Exception
                                                       sm.AddScene("TestScene", ts)
                                                   End Try
                                                   'ts.StartServer()
                                               End Sub

        With startButton
            .Text = "Start Server"
            .AutoSize = ButtonAutoSize.XY
            .Position = New Vector2(3, sender.GraphicsDevice.ScissorRectangle.Height - startButton.Size.Y - 3)
        End With
        '  AddElement(startButton)

        spectrum = New SpectrumAnalyser(Me)
        spectrum.Size = New Vector2(sender.GraphicsDevice.ScissorRectangle.Width, sender.GraphicsDevice.ScissorRectangle.Height)
        spectrum.FillColor = New Color(255, 255, 255)
        Dim lbc As New LoopbackCapture()
        Dim lbm As New LoopbackMonitor(lbc)
        spectrum.Source = lbm
        AddElement(spectrum)
        lbm.BeginMonitoring()
        AddHandler lbc.FftCalculated, AddressOf ChangeColorToWhite
        AddHandler lbm.OnSignalPeak, AddressOf OnSignalPeak
    End Sub
    Dim spectrum As SpectrumAnalyser
    ' Private currentTime As Long = Stopwatch.GetTimestamp
    'Private lastTime As New TimeSpan(Stopwatch.GetTimestamp)
    Private Sub ChangeColorToWhite(sender As Object, e As FftEventArgs)
        ' Dim currentTime As New TimeSpan(Stopwatch.GetTimestamp)

        '   If currentTime.TotalMilliseconds >= lastTime.TotalMilliseconds + 1 Then
        '   lastTime = currentTime

    End Sub

    Private Sub OnSignalPeak(signal As SignalRegister)
        If signal.Name = "bar" Then
            spectrum.FillColor = New Color(0, 0, 255)
        End If
    End Sub

    Private Sub EnterScene(p As Point)
        ts = New TestScene(sm)
        'ts.Username = Username.Text
        Try
            If sm.GetScene("TestScene") Is Nothing Then
                sm.AddScene("TestScene", ts)
            End If
        Catch ex As Exception
            sm.AddScene("TestScene", ts)
        End Try
        sm.ActiveScene = sm.GetScene("TestScene")
    End Sub

    ''' <summary>
    ''' Here is where you can add Initialization logic to your Scene.
    ''' </summary>
    ''' <remarks>This is most useful for classes that require object instances to be constructed.</remarks>
    Public Overrides Sub Initialize(gameTime As GameTime)
        MyBase.Initialize(gameTime)


    End Sub

End Class
