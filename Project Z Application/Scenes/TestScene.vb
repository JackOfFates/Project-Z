#Region "Using Statements"
Imports System.Collections.Generic
Imports System.Net.Sockets
Imports System.Runtime.Serialization.Formatters.Binary
Imports System.Timers
Imports MicroLibrary.Serialization
Imports Microsoft.Xna.Framework
Imports Microsoft.Xna.Framework.Content
Imports Microsoft.Xna.Framework.GamerServices
Imports Microsoft.Xna.Framework.Graphics
Imports Microsoft.Xna.Framework.Input
Imports Microsoft.Xna.Framework.Storage
Imports ProjectZ.Shared.Animations
Imports ProjectZ.Shared.Content
Imports ProjectZ.Shared.Drawing
Imports ProjectZ.Shared.Drawing.UI
Imports ProjectZ.Shared.Drawing.UI.Advanced
Imports ProjectZ.Shared.Drawing.UI.Input

#End Region

''' <summary>
''' Main Application Scene
''' </summary>
Public Class TestScene
    Inherits Scene

    'Dim bf As New MicroLibrary.Serialization.JsonSerializer

#Region "Server Stuff"

    Public ServerPort As Integer = 4552, isServer As Boolean = False

    Public Property ServerElements As New Dictionary(Of String, SceneElement)

    'Public WithEvents Server As MicroLibrary.Networking.Server.TcpServer

    'Private Sub Server_OnReceive(sender As Socket, obj As Object, BytesReceived As Integer) Handles Server.OnReceive
    '    Dim objType As Type = obj.GetType
    '    If objType.BaseType = GetType(SceneElement) Then
    '        Dim castedObj As SceneElement = DirectCast(obj, SceneElement)
    '        If ServerElements.ContainsKey(castedObj.GUID) Then
    '            ServerElements(castedObj.GUID).UpdateElement(castedObj)
    '            Server.SendBroadcast(castedObj)
    '        Else
    '            ServerElements.Add(castedObj.GUID, castedObj)
    '            Server.SendBroadcast(castedObj)
    '            'AddElement(castedObj) -- Server doesnt need UI
    '        End If
    '    ElseIf objType.IsAssignableFrom(GetType(RequestTicket)) Then
    '        Dim r As RequestTicket = DirectCast(obj, RequestTicket)
    '        Select Case r.Request
    '            Case "SYNC"
    '                SyncElementsWith(sender)
    '        End Select
    '    End If
    '    ' Update Server Info

    'End Sub

    'Private Sub Server_OnConnected(Sender As Socket) Handles Server.OnConnected
    '    SyncElementsWith(Sender)
    'End Sub

    'Public Sub SyncElementsWith(sender As Socket)
    '    Dim ServerObjsCopy As New List(Of SceneElement)
    '    ServerObjsCopy.AddRange(ServerElements.Values)
    '    For Each o As SceneElement In ServerObjsCopy
    '        Server.Send(CType(sender.RemoteEndPoint, System.Net.IPEndPoint), o)
    '    Next
    'End Sub

    'Public Sub StartServer()
    '    Try
    '        Server = New MicroLibrary.Networking.Server.TcpServer(bf, ServerPort)
    '        Server.Listen(100)
    '    Catch ex As Exception

    '    End Try
    'End Sub

#End Region

    '#Region "Client Stuff"

    '    Public Property Username As String = Guid.NewGuid.ToString

    '    Public Property StayConnected As Boolean = False

    '    Public Property LocalElements As New Dictionary(Of String, SceneElement)
    '    Private ChangedElements As New List(Of SceneElement)

    '    Public WithEvents Client As New MicroLibrary.Networking.Client.TcpClient(bf)

    '    Public WithEvents updateTimer As New Timers.Timer(16.7)

    '    Private Sub Client_OnReceive(sender As Socket, obj As Object, BytesReceived As Integer) Handles Client.OnReceive
    '        Dim objType As Type = obj.GetType
    '        If objType.BaseType = GetType(SceneElement) Then
    '            Dim castedObj As SceneElement = DirectCast(obj, SceneElement)
    '            If LocalElements.ContainsKey(castedObj.GUID) Then
    '                LocalElements(castedObj.GUID).UpdateElement(castedObj)
    '                AddElement(LocalElements(castedObj.GUID))
    '            Else
    '                LocalElements.Add(castedObj.GUID, castedObj)
    '                castedObj.updateScene(Me)
    '                AddElement(castedObj)
    '                ChangeCheck(castedObj)
    '            End If
    '        ElseIf objType.IsAssignableFrom(GetType(AuthenticatedResponse)) Then
    '            Dim r As AuthenticatedResponse = DirectCast(obj, AuthenticatedResponse)

    '        End If
    '    End Sub

    '    Private Sub ChangeCheck(sender As SceneElement)
    '        If sender IsNot Nothing Then AddHandler sender.UserInvalidated, Sub() If Not ChangedElements.Contains(sender) Then ChangedElements.Add(sender)
    '    End Sub


    '    Private Sub updateTimer_Elapsed(sender As Object, e As ElapsedEventArgs) Handles updateTimer.Elapsed
    '        If ChangedElements.Count > 0 Then
    '            Dim newlist As New List(Of SceneElement)
    '            newlist.Add(ChangedElements(0))
    '            Dim element As SceneElement = newlist(0)
    '            ChangedElements.RemoveAt(0)
    '            Client.Send(element)
    '        End If
    '    End Sub

    '    'Private Sub Client_OnConnected(Sender As Socket) Handles Client.OnConnected
    '    '    RefreshTimer()
    '    'End Sub

    '    'Private Sub Client_OnConnectionInterrupt(sender As Socket) Handles Client.OnConnectionInterrupt
    '    '    If StayConnected = True Then
    '    '        ReconnectClient()
    '    '    End If
    '    'End Sub

    '    Public Sub RefreshTimer()
    '        If updateTimer Is Nothing Then
    '            updateTimer = New Timers.Timer(16.7)
    '        End If
    '        updateTimer.Start()
    '    End Sub

    '    Public Sub stopTimer()
    '        If updateTimer IsNot Nothing Then
    '            updateTimer.Stop()
    '            updateTimer.Dispose()
    '            updateTimer = Nothing
    '        End If
    '    End Sub

    '    Public Sub ReconnectClient()
    '        Client.Connect("127.0.0.1", ServerPort)
    '    End Sub

    '#End Region

    ''' <summary>
    ''' Initialization of the Scene.
    ''' </summary>
    ''' <remarks>Elements can be added here.</remarks>
    Public Sub New(ByRef SceneManager As SceneManager)
        MyBase.New(SceneManager)
        Me.isCursorVisible = True
    End Sub

    ''' <summary>
    ''' Here is where you can add Initialization logic to your Scene.
    ''' </summary>
    ''' <remarks>This is most useful for classes that require object instances to be constructed.</remarks>
    Public Overrides Sub Initialize(gameTime As GameTime)
        MyBase.Initialize(gameTime)
        ' StayConnected = True

        '  ReconnectClient()
        isCursorVisible = True
        Me.sender.IsMouseVisible = False
        'Spawn Box Button
        Dim SpawnButton As New UI.Input.Button(Me)
        SpawnButton.AutoSize = ButtonAutoSize.XY
        SpawnButton.Padding = New Thickness(6)
        SpawnButton.Text = "Spawn Random Rectangle"
        SpawnButton.Position = New Vector2(3, sender.GraphicsDevice.ScissorRectangle.Height - SpawnButton.Size.Y - 3)
        AddHandler SpawnButton.MouseLeftUp, Sub() SpawnRandomBoxes()
        ' AddElement(SpawnButton)
    End Sub

    Public Sub SpawnRandomBoxes()
        Dim boxes As New List(Of UI.Primitives.RectangleElement)
        Dim cx As Integer = 0, cy As Integer = 0, nh As Integer = r.Next(13, 167)
        For i As Integer = 0 To 1000
            Dim b As New Primitives.RectangleElement(Me)
            With b
                .Size = New Vector2(r.Next(13, 167), r.Next(13, 167))
                .isMovable = True
                .Position = New Vector2(cx, cy)
                .BackgroundColor = New Color(r.Next(0, 256), r.Next(0, 256), r.Next(0, 256))
            End With
            cx += CInt(b.Size.X)
            If cx > graphicsDevice.ScissorRectangle.Width Then
                cy += nh
                cx = 0
                nh = r.Next(13, 167)
            End If

            'Client.Send(b)
            If cy + nh >= graphicsDevice.ScissorRectangle.Height Then
                Exit For
            End If
        Next
    End Sub

    Dim CursorDefault As Vector2() = {New Vector2(1, 1), New Vector2(3, 10), New Vector2(5, 5), New Vector2(9, 5)}
    Dim lastEvent As Double = 0
    Dim myCursor As PolygonElement
    Private Sub TestScene_OnMouseMove(currentPoint As Point, lastPoint As Point) Handles Me.OnMouseMove
        If gameTime IsNot Nothing AndAlso gameTime.TotalGameTime.TotalMilliseconds > lastEvent + 33.3 Then
            lastEvent = gameTime.TotalGameTime.TotalMilliseconds
            ' Client.Send(Cursor)
        End If
    End Sub

    Dim r As New Random

End Class
