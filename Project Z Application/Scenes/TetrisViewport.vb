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
Public Class TetrisViewport
    Inherits Scene

    Private TetrisGame As TetrisElement

    ''' <summary>
    ''' Initialization of the Scene.
    ''' </summary>
    ''' <remarks>Elements can be added here.</remarks>
    Public Sub New(ByRef SceneManager As SceneManager)
        MyBase.New(SceneManager)
        Me.isCursorVisible = True
        Me.BackgroundColor = New Color(10, 10, 10)

        ' Create the Tetris game element (12 columns x 24 rows, 24px cells)
        TetrisGame = New TetrisElement(Me, 12, 24, 24)

        ' Center the Tetris board on screen
        Dim screenWidth As Integer = Me.sender.GraphicsDevice.Viewport.Width
        Dim screenHeight As Integer = Me.sender.GraphicsDevice.Viewport.Height
        TetrisGame.Position = New Vector2(
            CSng((screenWidth - TetrisGame.Size.X) / 2),
            CSng((screenHeight - TetrisGame.Size.Y) / 2))

        ' Add to scene
        AddElement(TetrisGame)
    End Sub

    ''' <summary>
    ''' Here is where you can add Initialization logic to your Scene.
    ''' </summary>
    ''' <remarks>This is most useful for classes that require object instances to be constructed.</remarks>
    Public Overrides Sub Initialize(gameTime As GameTime)
        MyBase.Initialize(gameTime)

        isCursorVisible = True
        Me.sender.IsMouseVisible = True
    End Sub

End Class
