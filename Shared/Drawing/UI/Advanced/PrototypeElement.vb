Imports Microsoft.Xna.Framework
Imports Microsoft.Xna.Framework.Graphics
Imports ProjectZ.Shared.Animations.Easing
Imports ProjectZ.Shared.Animations.Properties
Imports ProjectZ.Shared.Content
Imports ProjectZ.Shared.Drawing.UI.Primitives


Namespace [Shared].Drawing.UI.Advanced

    <Serializable>
    Public Class PrototypeElement
        Inherits UI.Input.Button

#Region "Properties"

        Public Property Target As SceneElement

#End Region

#Region "Child Elements"


#End Region

#Region "Constructors"

        Public Sub New(Scene As Scene, Target As SceneElement)
            MyBase.New(Scene)
            Init(Target)
        End Sub

        Public Sub New(Scene As Scene, Target As SceneElement, spriteBatch As SpriteBatch)
            MyBase.New(Scene, spriteBatch)
            Init(Target)
        End Sub

        Public Sub New(Scene As Scene, Target As SceneElement, newSpritebatch As Boolean)
            MyBase.New(Scene, newSpritebatch)
            Init(Target)
        End Sub

        Private Sub Init(Target As SceneElement)
            Me.isPrototype = True
            Me.Target = Target
            ' Add Children
            'Children.Add()
            Clip = True
            isVisible = False
            Me.Children.ForEach(Sub(c) c.isVisible = False)
        End Sub

#End Region

    End Class


End Namespace