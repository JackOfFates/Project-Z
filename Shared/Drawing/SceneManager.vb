Imports System.Collections.Generic
Imports Microsoft.Xna.Framework
Imports Microsoft.Xna.Framework.Input
Imports ProjectZ.Shared.Content

#If WINDOWS Then
Imports ProjectZ.Windows.Input
#End If

Namespace [Shared].Drawing

    Public Class SceneManager
        Implements IDisposable

#Region "Properties"

#If WINDOWS Then
        Public Property UseHardwareInput As Boolean
            Get
                Return _UseHardwareInput
            End Get
            Set(value As Boolean)
                _UseHardwareInput = value
                If _UseHardwareInput Then
                    MouseHook = New Windows.Input.MouseHook()
                    MouseHook.Install()
                ElseIf MouseHook IsNot Nothing Then
                    MouseHook.Uninstall()
                    MouseHook = Nothing
                End If
            End Set
        End Property
        Private _UseHardwareInput As Boolean = False
#End If

        Public Property LimitFPS As Integer
            Get
                Return _LimitFPS
            End Get
            Set(value As Integer)
                _LimitFPS = Math.Max(Math.Min(value, 1000), 1)
                SetLimit = True
            End Set
        End Property
        Private _LimitFPS As Integer = 60
        Private SetLimit As Boolean = False
        Private Property isDragging As Boolean = False
        Private Property Scenes As New Dictionary(Of String, Scene)
        Public Property ActiveScene As Scene
        Public Property Sender As Game

#End Region

#Region "Input"

#Region "Keypress"

        Private LastKeyboardState As KeyboardState = Keyboard.GetState
        Private LastKeyHoldTick As Long = 0
        Private LastKeyProcessTick As Long = 0
        Private Const OneSecond As Long = 10000000

        Private DoDuplicate As Boolean = False
        Private DuplicateKey As Keys

        Private Sub DetectKeyPress(gameTime As GameTime)
            Dim KeyboardState As KeyboardState = Keyboard.GetState
            Dim currentHoldTick As Long = gameTime.TotalGameTime.Ticks

            For i As Integer = 0 To 254
                Dim Key As Keys = CType(i, Keys)
                If KeyboardState.IsKeyDown(Key) And LastKeyboardState.IsKeyUp(Key) Then
                    LastKeyHoldTick = gameTime.TotalGameTime.Ticks
                    ActiveScene.KeyDown(Key, KeyboardState)
                    DoDuplicate = False
                    Exit For
                ElseIf KeyboardState.IsKeyUp(Key) And LastKeyboardState.IsKeyDown(Key) Then
                    LastKeyHoldTick = gameTime.TotalGameTime.Ticks
                    ActiveScene.KeyUp(Key, KeyboardState)
                    ProcessKeyPress(Key, KeyboardState)
                    DoDuplicate = False
                    Exit For
                ElseIf DoDuplicate AndAlso KeyboardState.IsKeyDown(Key) AndAlso currentHoldTick > LastKeyProcessTick + 2000000 Then
                    ProcessKeyPress(DuplicateKey, LastKeyboardState)
                    Exit For
                ElseIf (currentHoldTick > LastKeyHoldTick + 5000000) AndAlso (KeyboardState.IsKeyDown(Key) And LastKeyboardState.IsKeyDown(Key)) Then
                    LastKeyHoldTick = gameTime.TotalGameTime.Ticks
                    LastKeyProcessTick = LastKeyHoldTick
                    DuplicateKey = Key
                    DoDuplicate = True
                    Exit For
                End If
            Next

            LastKeyboardState = KeyboardState
        End Sub

        Private Sub ProcessKeyPress(PressedKey As Keys, keyboardState As KeyboardState)
            If DebugEnabled Then
                Select Case PressedKey
                    Case Keys.F1
                        DebugParams.ShowFPS = Not DebugParams.ShowFPS
                    Case Keys.F2
                        DebugParams.ShowDrawFPS = Not DebugParams.ShowDrawFPS
                    Case Keys.F3

                    Case Keys.F4

                    Case Keys.F5

                    Case Keys.OemTilde
                        ConsoleEnabled = Not ConsoleEnabled
                End Select
            End If
            ActiveScene.KeyPress(PressedKey, keyboardState)
            PressedKey = Nothing
        End Sub

        Public Shared Function TryConvertKeyboardInput(key As Keys, keyboard As KeyboardState) As String
            Dim ReturnString As String = String.Empty
            Dim shift As Boolean = (keyboard.IsKeyDown(Keys.LeftShift) Or keyboard.IsKeyDown(Keys.RightShift))

            Select Case key
                Case Keys.Enter
                    ReturnString = ReturnString & Environment.NewLine
                    'Alphabet keys
                Case Keys.A
                    If shift Then
                        ReturnString = ReturnString & "A"
                    Else
                        ReturnString = ReturnString & "a"
                    End If
                Case Keys.B
                    If shift Then
                        ReturnString = ReturnString & "B"
                    Else
                        ReturnString = ReturnString & "b"
                    End If
                Case Keys.C
                    If shift Then
                        ReturnString = ReturnString & "C"
                    Else
                        ReturnString = ReturnString & "c"
                    End If
                Case Keys.D
                    If shift Then
                        ReturnString = ReturnString & "D"
                    Else
                        ReturnString = ReturnString & "d"
                    End If

                Case Keys.E
                    If shift Then
                        ReturnString = ReturnString & "E"
                    Else
                        ReturnString = ReturnString & "e"
                    End If
                Case Keys.F
                    If shift Then
                        ReturnString = ReturnString & "F"
                    Else
                        ReturnString = ReturnString & "f"
                    End If
                Case Keys.G
                    If shift Then
                        ReturnString = ReturnString & "G"
                    Else
                        ReturnString = ReturnString & "g"
                    End If

                Case Keys.H
                    If shift Then
                        ReturnString = ReturnString & "H"
                    Else
                        ReturnString = ReturnString & "h"
                    End If
                Case Keys.I
                    If shift Then
                        ReturnString = ReturnString & "I"
                    Else
                        ReturnString = ReturnString & "i"
                    End If
                Case Keys.J
                    If shift Then
                        ReturnString = ReturnString & "J"
                    Else
                        ReturnString = ReturnString & "j"
                    End If
                Case Keys.K
                    If shift Then
                        ReturnString = ReturnString & "K"
                    Else
                        ReturnString = ReturnString & "k"
                    End If
                Case Keys.L
                    If shift Then
                        ReturnString = ReturnString & "L"
                    Else
                        ReturnString = ReturnString & "l"
                    End If
                Case Keys.M
                    If shift Then
                        ReturnString = ReturnString & "M"
                    Else
                        ReturnString = ReturnString & "m"
                    End If
                Case Keys.N
                    If shift Then
                        ReturnString = ReturnString & "N"
                    Else
                        ReturnString = ReturnString & "n"
                    End If
                Case Keys.O
                    If shift Then
                        ReturnString = ReturnString & "O"
                    Else
                        ReturnString = ReturnString & "o"
                    End If
                Case Keys.P
                    If shift Then
                        ReturnString = ReturnString & "P"
                    Else
                        ReturnString = ReturnString & "p"
                    End If
                Case Keys.Q
                    If shift Then
                        ReturnString = ReturnString & "Q"
                    Else
                        ReturnString = ReturnString & "q"
                    End If
                Case Keys.R
                    If shift Then
                        ReturnString = ReturnString & "R"
                    Else
                        ReturnString = ReturnString & "r"
                    End If
                Case Keys.S
                    If shift Then
                        ReturnString = ReturnString & "S"
                    Else
                        ReturnString = ReturnString & "s"
                    End If
                Case Keys.T
                    If shift Then
                        ReturnString = ReturnString & "T"
                    Else
                        ReturnString = ReturnString & "t"
                    End If
                Case Keys.U
                    If shift Then
                        ReturnString = ReturnString & "U"
                    Else
                        ReturnString = ReturnString & "u"
                    End If
                Case Keys.V
                    If shift Then
                        ReturnString = ReturnString & "V"
                    Else
                        ReturnString = ReturnString & "v"
                    End If
                Case Keys.W
                    If shift Then
                        ReturnString = ReturnString & "W"
                    Else
                        ReturnString = ReturnString & "w"
                    End If
                Case Keys.X
                    If shift Then
                        ReturnString = ReturnString & "X"
                    Else
                        ReturnString = ReturnString & "x"
                    End If
                Case Keys.Y
                    If shift Then
                        ReturnString = ReturnString & "Y"
                    Else
                        ReturnString = ReturnString & "y"
                    End If
                Case Keys.Z
                    If shift Then
                        ReturnString = ReturnString & "Z"
                    Else
                        ReturnString = ReturnString & "z"
                    End If
                    'Decimal keys
                Case Keys.D0
                    If shift Then
                        ReturnString = ReturnString & ")"
                    Else
                        ReturnString = ReturnString & "0"
                    End If
                Case Keys.D1
                    If shift Then
                        ReturnString = ReturnString & "!"
                    Else
                        ReturnString = ReturnString & "1"
                    End If
                Case Keys.D2
                    If shift Then
                        ReturnString = ReturnString & "@"
                    Else
                        ReturnString = ReturnString & "2"
                    End If

                Case Keys.D3
                    If shift Then
                        ReturnString = ReturnString & "#"
                    Else
                        ReturnString = ReturnString & "3"
                    End If
                Case Keys.D4
                    If shift Then
                        ReturnString = ReturnString & "$"
                    Else
                        ReturnString = ReturnString & "4"
                    End If
                Case Keys.D5
                    If shift Then
                        ReturnString = ReturnString & "%"
                    Else
                        ReturnString = ReturnString & "5"
                    End If
                Case Keys.D6
                    If shift Then
                        ReturnString = ReturnString & "^"
                    Else
                        ReturnString = ReturnString & "6"
                    End If
                Case Keys.D7
                    If shift Then
                        ReturnString = ReturnString & "&"
                    Else
                        ReturnString = ReturnString & "7"
                    End If
                Case Keys.D8
                    If shift Then
                        ReturnString = ReturnString & "*"
                    Else
                        ReturnString = ReturnString & "8"
                    End If
                Case Keys.D9
                    If shift Then
                        ReturnString = ReturnString & "("
                    Else
                        ReturnString = ReturnString & "9"
                    End If
                    'Decimal numpad keys
                Case Keys.NumPad0
                    ReturnString = ReturnString & "0"
                Case Keys.NumPad1
                    ReturnString = ReturnString & "1"
                Case Keys.NumPad2
                    ReturnString = ReturnString & "2"
                Case Keys.NumPad3
                    ReturnString = ReturnString & "3"
                Case Keys.NumPad4
                    ReturnString = ReturnString & "4"
                Case Keys.NumPad5
                    ReturnString = ReturnString & "5"
                Case Keys.NumPad6
                    ReturnString = ReturnString & "6"
                Case Keys.NumPad7
                    ReturnString = ReturnString & "7"
                Case Keys.NumPad8
                    ReturnString = ReturnString & "8"
                Case Keys.NumPad9
                    ReturnString = ReturnString & "9"
                    'Special keys
                Case Keys.OemTilde
                    If shift Then
                        ReturnString = ReturnString & "~"
                    Else
                        ReturnString = ReturnString & "`"
                    End If
                Case Keys.OemSemicolon
                    If shift Then
                        ReturnString = ReturnString & ":"
                    Else
                        ReturnString = ReturnString & ";"
                    End If
                Case Keys.OemQuotes
                    If shift Then
                        ReturnString = ReturnString & """"
                    Else
                        ReturnString = ReturnString & "'"
                    End If
                Case Keys.OemQuestion
                    If shift Then
                        ReturnString = ReturnString & "?"
                    Else
                        ReturnString = ReturnString & "/"
                    End If
                Case Keys.OemPlus
                    If shift Then
                        ReturnString = ReturnString & "+"
                    Else
                        ReturnString = ReturnString & "="
                    End If
                Case Keys.OemPipe
                    If shift Then
                        ReturnString = ReturnString & "|"
                    Else
                        ReturnString = ReturnString & "\"
                    End If
                Case Keys.OemPeriod
                    If shift Then
                        ReturnString = ReturnString & ">"
                    Else
                        ReturnString = ReturnString & "."
                    End If
                Case Keys.OemOpenBrackets
                    If shift Then
                        ReturnString = ReturnString & "{"
                    Else
                        ReturnString = ReturnString & "["
                    End If
                Case Keys.OemCloseBrackets
                    If shift Then
                        ReturnString = ReturnString & "}"
                    Else
                        ReturnString = ReturnString & "]"
                    End If
                Case Keys.OemMinus
                    If shift Then
                        ReturnString = ReturnString & "_"
                    Else
                        ReturnString = ReturnString & "-"
                    End If
                Case Keys.OemComma
                    If shift Then
                        ReturnString = ReturnString & "<"
                    Else
                        ReturnString = ReturnString & ","
                    End If
                Case Keys.Space
                    ReturnString = ReturnString & " "
            End Select

            Return ReturnString
        End Function

#End Region

#Region "Mouse"

        Private LastState As MouseState = Nothing
        Private StartPoint As Point = Nothing
        Private MouseDown As Boolean = False
        Private DragThreshold As Integer = 2

#If WINDOWS Then

        Private WithEvents MouseHook As MouseHook
        Private LastPoint As Point = Nothing

        ' Windows-specific: Get actual titlebar and border sizes from System.Windows.Forms
        Private ReadOnly TitlebarHeight As Integer = System.Windows.Forms.SystemInformation.CaptionHeight +
                                                     System.Windows.Forms.SystemInformation.Border3DSize.Height

        Private ReadOnly BorderWidth As Integer = System.Windows.Forms.SystemInformation.Border3DSize.Width +
                                                  System.Windows.Forms.SystemInformation.BorderSize.Width
#Else
#Disable Warning IDE0051, IDE0052 ' Suppress unused member warnings - these are platform stubs
        ' Non-Windows platforms: Default to 0 (no window chrome offset needed)
        Private ReadOnly TitlebarHeight As Integer = 0
        Private ReadOnly BorderWidth As Integer = 0
#Enable Warning IDE0051, IDE0052
#End If

#If WINDOWS Then
        Private Function ToRelativePoint(ms As MouseHook.MSLLHOOKSTRUCT) As Point
            Dim clientPoint As Point
            If Sender.Window.IsBorderless Then
                clientPoint = New Point(ms.pt.x, ms.pt.y).Subtract(Sender.Window.ClientBounds.Location)
            Else
                clientPoint = New Point(ms.pt.x - BorderWidth, ms.pt.y - TitlebarHeight).Subtract(Sender.Window.ClientBounds.Location)
            End If
            ' Scale to back buffer coordinates
            Return ScaleMousePosition(clientPoint)
        End Function

        Private Sub MouseHook_LeftButtonDown(mouseStruct As MouseHook.MSLLHOOKSTRUCT) Handles MouseHook.LeftButtonDown
            StartPoint = ToRelativePoint(mouseStruct)
            MouseDown = True
            ActiveScene.MouseLeftDown(StartPoint)
        End Sub

        Private Sub MouseHook_LeftButtonUp(mouseStruct As MouseHook.MSLLHOOKSTRUCT) Handles MouseHook.LeftButtonUp
            Dim p As Point = ToRelativePoint(mouseStruct)
            MouseDown = False
            ActiveScene.MouseLeftUp(p)
            Dim Difference As Point = GetDifference(StartPoint, p)
            If (Difference.X > DragThreshold) Or (Difference.Y > DragThreshold) Then
                ' Mouse was dragged
                ActiveScene.MouseDragDrop(p, StartPoint)
            Else
                ' Mouse was clicked
                ActiveScene.MouseLeftClick(p)
            End If
        End Sub

        Private Sub MouseHook_MouseMove(mouseStruct As MouseHook.MSLLHOOKSTRUCT) Handles MouseHook.MouseMove
            Dim p As Point = ToRelativePoint(mouseStruct)
            If p <> LastPoint Then
                If MouseDown Then
                    Dim Difference As Point = GetDifference(StartPoint, p)
                    If isDragging OrElse ((Difference.X > DragThreshold) Or (Difference.Y > DragThreshold)) Then
                        ' Mouse was dragged
                        isDragging = True
                        ActiveScene.MouseDrag(p, StartPoint)
                    End If
                ElseIf isDragging Then
                    isDragging = False
                End If
                ActiveScene.MouseMove(p, LastPoint)
            End If
        End Sub

        Private Sub MouseHook_RightButtonDown(mouseStruct As MouseHook.MSLLHOOKSTRUCT) Handles MouseHook.RightButtonDown
            ActiveScene.MouseRightClick(ToRelativePoint(mouseStruct))
        End Sub

#End If

        Private Sub DetectMouseEvents(gameTime As GameTime)
            Dim State As MouseState = Mouse.GetState()
            Dim scaledPosition As Point = ScaleMousePosition(State.Position)
            Dim scaledState As New MouseState(scaledPosition.X, scaledPosition.Y, State.ScrollWheelValue,
                                              State.LeftButton, State.MiddleButton, State.RightButton,
                                              State.XButton1, State.XButton2)
            ActiveScene.SetMouseState(scaledState)

            'Left Button
            If (LastState.LeftButton = ButtonState.Pressed) And (State.LeftButton = ButtonState.Released) Then
                MouseDown = False
                ActiveScene.MouseLeftUp(scaledPosition)
                Dim Difference As Point = GetDifference(StartPoint, scaledPosition)
                If (Difference.X > DragThreshold) Or (Difference.Y > DragThreshold) Then
                    ' Mouse was dragged
                    ActiveScene.MouseDragDrop(scaledPosition, StartPoint)
                Else
                    ' Mouse was clicked
                    ActiveScene.MouseLeftClick(scaledPosition)
                End If
            ElseIf (State.LeftButton = ButtonState.Pressed) And Not MouseDown Then
                StartPoint = scaledPosition
                MouseDown = True
                ActiveScene.MouseLeftDown(scaledPosition)
            End If

            'Right Button
            If LastState.RightButton = ButtonState.Pressed Then
                ' Mouse was clicked
                ActiveScene.MouseRightClick(scaledPosition)
            End If

            If scaledPosition <> LastState.Position Then
                If MouseDown Then
                    Dim Difference As Point = GetDifference(StartPoint, scaledPosition)
                    If isDragging OrElse ((Difference.X > DragThreshold) Or (Difference.Y > DragThreshold)) Then
                        ' Mouse was dragged
                        isDragging = True
                        ActiveScene.MouseDrag(scaledPosition, StartPoint)
                    End If
                ElseIf isDragging Then
                    isDragging = False
                End If
                ActiveScene.MouseMove(scaledPosition, LastState.Position)
            End If

            LastState = scaledState
        End Sub

        Private Function ScaleMousePosition(position As Point) As Point
            ' Get the actual rendering target size (back buffer)
            Dim backBufferWidth As Integer = Sender.GraphicsDevice.PresentationParameters.BackBufferWidth
            Dim backBufferHeight As Integer = Sender.GraphicsDevice.PresentationParameters.BackBufferHeight

            ' Get the window's client area size
            Dim clientBounds As Rectangle = Sender.Window.ClientBounds

            ' Avoid division by zero
            If clientBounds.Width <= 0 OrElse clientBounds.Height <= 0 Then Return position

            ' If back buffer matches client bounds, no scaling needed
            If backBufferWidth = clientBounds.Width AndAlso backBufferHeight = clientBounds.Height Then
                Return position
            End If

            ' Mouse position is in window client coordinates
            ' We need to scale it to match the back buffer coordinates
            Dim scaleX As Single = CSng(backBufferWidth) / CSng(clientBounds.Width)
            Dim scaleY As Single = CSng(backBufferHeight) / CSng(clientBounds.Height)

            Return New Point(CInt(position.X * scaleX), CInt(position.Y * scaleY))
        End Function

        Private Function GetDifference(StartPoint As Point, EndPoint As Point) As Point
            Dim DifferenceX As Integer = 0, DifferenceY As Integer = 0
            If StartPoint.X < EndPoint.X Then
                DifferenceX = EndPoint.X - StartPoint.X
            Else
                DifferenceX = StartPoint.X - EndPoint.X
            End If
            If StartPoint.Y < EndPoint.Y Then
                DifferenceY = EndPoint.Y - StartPoint.Y
            Else
                DifferenceY = StartPoint.Y - EndPoint.Y
            End If
            Return New Point(DifferenceX, DifferenceY)
        End Function

#End Region

#End Region

#Region "Debugging"

#Region "Debug Properties"

        Public Property DebugEnabled As Boolean = False
        Public Property ConsoleEnabled As Boolean = False
        Public Property DebugParams As New DebugParameters

        Private _DrawFPS As Integer = 0
        Private LastDrawTick As Long = 0
#End Region

        Private I_DrawFPS As Integer = 0
        Private LastDebugText As String = ""
        Private DebugHeaderRectangle As Rectangle = Nothing
        Private DebugPosition As New Vector2(4, 4)
        Private DebugTextPosition As Vector2 = Nothing
        Private DebugHeaderSize As Vector2 = Nothing
        Private DebugRectangle As Rectangle = Nothing
        Private DebugHeaderColor As Color = Color.LimeGreen
        Private SmallFont As String = Fonts.SegoeUI.GetResourceName(10)
        Private RegularFont As String = Fonts.SegoeUI.GetResourceName(12)
        Private LargeFont As String = Fonts.SegoeUI.GetResourceName(18)

        Private Sub DrawDebugText(gameTime As GameTime)
            If DebugHeaderSize = Nothing Then
                DebugHeaderSize = ActiveScene.MeasureText(LargeFont, Sender.Window.Title)
            End If
            If DebugTextPosition = Nothing Then
                DebugTextPosition = New Vector2(DebugPosition.X, DebugPosition.Y + DebugHeaderSize.Y)
            End If

            Dim DebugText As String = String.Empty

            If DebugParams.ShowFPS Then
                DebugText += String.Format("Tick FPS: {1:D3}{0}", {Environment.NewLine, ActiveScene.FPS})
            End If

            If DebugParams.ShowDrawFPS Then
                DebugText += String.Format("Draw FPS: {1}{0}", {Environment.NewLine, ActiveScene.DrawFPS})
            End If

            If DebugText.EndsWith(Environment.NewLine) Then
                DebugText = DebugText.Remove(DebugText.Length - 1).Trim
            ElseIf DebugText.StartsWith(Environment.NewLine) Then
                DebugText = DebugText.Remove(0, 1).Trim
            End If

            If DebugRectangle = Nothing Or LastDebugText <> DebugText Then
                Dim DebugTextSize As Vector2 = ActiveScene.MeasureText(RegularFont, DebugText)
                Dim LargestWidth As Integer = CInt(If(DebugHeaderSize.X > DebugTextSize.X, DebugHeaderSize.X, DebugTextSize.X))
                Dim RectHeight As Integer = CInt(DebugTextSize.Y)
                DebugRectangle = New Rectangle(CInt(DebugTextPosition.X), CInt(DebugTextPosition.Y), LargestWidth, RectHeight)
            End If

            If DebugHeaderRectangle = Nothing Then
                DebugHeaderRectangle = New Rectangle(CInt(DebugPosition.X), CInt(DebugPosition.Y), DebugRectangle.Width, CInt(DebugHeaderSize.Y))
            End If
            'ActiveScene.spriteBatch.Begin(Graphics.SpriteSortMode.Immediate, Graphics.BlendState.Opaque)
            'ActiveScene.spriteBatch.Draw(ActiveScene.WhitePlain, DebugHeaderRectangle, New Color(40, 40, 40))
            'ActiveScene.spriteBatch.DrawString(ActiveScene.contentCollection.Fonts(LargeFont), Sender.Window.Title, DebugPosition, DebugHeaderColor)

            'If DebugText <> String.Empty Then
            '    ActiveScene.spriteBatch.Draw(ActiveScene.WhitePlain, DebugRectangle, New Color(20, 20, 20))
            '    ActiveScene.spriteBatch.DrawString(ActiveScene.contentCollection.Fonts(RegularFont), DebugText, DebugTextPosition, Color.Snow)
            'End If
            'ActiveScene.spriteBatch.End()
        End Sub

#End Region

        Public Sub Draw(gameTime As GameTime)
            If ActiveScene IsNot Nothing Then
                ActiveScene.Draw(gameTime)
                If DebugEnabled Then
                    I_DrawFPS += 1
                    If (LastDrawTick + OneSecond) < gameTime.TotalGameTime.Ticks Then
                        LastDrawTick = gameTime.TotalGameTime.Ticks
                        _DrawFPS = I_DrawFPS
                        ActiveScene.DrawFPS = _DrawFPS
                        I_DrawFPS = 0
                    End If
                    DrawDebugText(gameTime)
                End If
            End If
        End Sub

        Public Sub Tick(gameTime As GameTime)
            If SetLimit Then
                ' FIX
                'gameTime.ElapsedGameTime =' TimeSpan.FromMilliseconds(1000 / LimitFPS)
            End If

            If ActiveScene IsNot Nothing Then
                DetectKeyPress(gameTime)
#If WINDOWS Then
                If Not UseHardwareInput Then
                    DetectMouseEvents(gameTime)
                End If
#ElseIf LINUX Then
                DetectMouseEvents(gameTime)
#End If
                ActiveScene.Tick(gameTime)
            End If
        End Sub

        Public Sub AddScene(Name As String, Scene As Scene)
            If Name.Trim = String.Empty Then
                Throw New Exception("Invalid Scene Name.")
            ElseIf Scenes.ContainsKey(Name) Then
                Throw New Exception("The Scene Manager already contains Scene Name '" & Name & "'.")
            Else
                Scenes.Add(Name, Scene)
            End If
        End Sub

        Public Sub RemoveScene(SceneName As String)
            Scenes.Remove(SceneName)
        End Sub

        Public Function GetScene(SceneName As String) As Scene
            Return Scenes(SceneName)
        End Function

        Public Sub New(sender As Game, LimitFPS As Integer)
            Me.Sender = sender
            Me.LimitFPS = LimitFPS
        End Sub

        Public Sub New(sender As Game)
            Me.Sender = sender
            Dim DefaultScene As New DefaultScene(Me)
            Me.AddScene("Default", DefaultScene)
            Me.ActiveScene = DefaultScene
        End Sub

        Public Class DebugParameters

            Public Property ShowFPS As Boolean = True
            Public Property ShowDrawFPS As Boolean = True

        End Class

#Region "IDisposable Support"
        Private disposedValue As Boolean

        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not disposedValue Then
                If disposing Then
                    ' Dispose all scenes
                    For Each scene In Scenes.Values
                        scene?.Dispose()
                    Next
                    Scenes.Clear()
                    ActiveScene = Nothing

#If WINDOWS Then
                    ' Uninstall mouse hook if installed
                    If MouseHook IsNot Nothing Then
                        MouseHook.Uninstall()
                        MouseHook = Nothing
                    End If
#End If
                End If
                disposedValue = True
            End If
        End Sub

        Public Sub Dispose() Implements IDisposable.Dispose
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub
#End Region

    End Class

End Namespace
