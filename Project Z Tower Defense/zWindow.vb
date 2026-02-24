#Region "Using Statements"
Imports Microsoft.Xna.Framework
Imports Microsoft.Xna.Framework.Graphics
Imports ProjectZ.Shared.Drawing
Imports SocketJack
Imports System.Threading

#End Region

''' <summary>
''' Game Window that MonoGame will use to display your application.
''' </summary>
''' <remarks></remarks>
Partial Public Class zWindow
    Inherits Game

    Dim oldBounds As Rectangle

    Protected Sub SetupSceneManager()
        ' Initialize the Scene Manager With Desired FPS
        sceneManager = New SceneManager(Me, 60)

        ' Create The Main Scene
        Dim mainScene As New MainScene(sceneManager)
        sceneManager.AddScene("MainScene", mainScene)

        ' This is how your application knows what to draw and when to draw it.
        sceneManager.ActiveScene = mainScene
    End Sub

    Protected Overrides Sub UnloadContent()
        ' Dispose the scene manager and all its scenes (including networking threads)
        If sceneManager IsNot Nothing Then
            sceneManager.Dispose()
            sceneManager = Nothing
        End If
    End Sub

    Public Sub New()
        MyBase.New()

        ' Set XNA Window Properties
        Size = New Point(1280, 720)
        graphics = New GraphicsDeviceManager(Me) With {
            .PreferMultiSampling = True,
            .SynchronizeWithVerticalRetrace = True,
            .PreferredBackBufferHeight = Size.Y,
            .PreferredBackBufferWidth = Size.X,
            .GraphicsProfile = GraphicsProfile.HiDef}
        oldBounds = Window.ClientBounds
        IsMouseVisible = True
        Content.RootDirectory = "Content"
        AddHandler Window.ClientSizeChanged, AddressOf HandleClientSizeChanged
    End Sub

    Private Sub HandleClientSizeChanged(sender As Object, e As EventArgs)
        If graphics Is Nothing Then Return
        Dim clientBounds As Rectangle = Window.ClientBounds
        If oldBounds.Width = clientBounds.Width AndAlso oldBounds.Height = clientBounds.Height Then Return
        oldBounds = clientBounds
        Size = New Point(clientBounds.Width, clientBounds.Height)
        graphics.PreferredBackBufferWidth = clientBounds.Width
        graphics.PreferredBackBufferHeight = clientBounds.Height
        graphics.ApplyChanges()
    End Sub

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
        ThreadManager.Shutdown()
    End Sub

    Protected Overrides Sub Dispose(disposing As Boolean)
        If disposing Then
            ' Ensure scene manager is disposed
            If sceneManager IsNot Nothing Then
                sceneManager.Dispose()
                sceneManager = Nothing
            End If
        End If
        MyBase.Dispose(disposing)
    End Sub
End Class
