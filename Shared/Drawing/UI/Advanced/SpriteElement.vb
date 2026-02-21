Imports Microsoft.Xna.Framework
Imports Microsoft.Xna.Framework.Graphics
Imports ProjectZ.Shared.Animations
Imports System.Collections.Generic
Imports ProjectZ.Shared.Animations.Properties
Imports ProjectZ.Shared.Content
Imports ProjectZ.Shared.Drawing.UI
Imports ProjectZ.Shared.Animations.Easing

Namespace [Shared].Drawing.UI.Advanced

    Public Class SpriteElement
        Inherits SceneElement

#Region "Properties"

#Region "Internals"
        Private CachedInterval As Double = 1000 / FPS

        Private Function GetCurrentFrame() As Integer
            Dim ReturnValue As Integer = 0
            If InternalAnimation IsNot Nothing Then
                If AnimationEnabled Then
                    ReturnValue = CInt(InternalAnimation.Value)
                Else
                    ReturnValue = CInt(InternalAnimation.lastValue)
                End If
            End If
            Return ReturnValue
        End Function

#End Region

#Region "Sprite Status Constants"
        Public Class SpriteStatus
            Public Const Idle As Byte = 0
            Public Const Active As Byte = 1
        End Class
#End Region

        Public Property FPS As Double
            Get
                Return _FPS
            End Get
            Set(value As Double)
                _FPS = value
                CachedInterval = 1000 / _FPS
                If InternalAnimation IsNot Nothing Then
                    InternalAnimation.Duration = TimeSpan.FromMilliseconds(CachedInterval)
                End If
            End Set
        End Property
        Private _FPS As Double = 60

        Public Property AutoRepeat As Boolean
            Get
                If InternalAnimation IsNot Nothing Then
                    Return InternalAnimation.AutoRepeat
                Else
                    Return _AutoRepeat
                End If
            End Get
            Set(value As Boolean)
                If InternalAnimation IsNot Nothing Then
                    InternalAnimation.AutoRepeat = value
                End If
                _AutoRepeat = value
            End Set
        End Property
        Private _AutoRepeat As Boolean = False

        Public Property AnimationEnabled As Boolean
            Get
                Return _AnimationEnabled
            End Get
            Set(value As Boolean)
                _AnimationEnabled = value
                Select Case _AnimationEnabled
                    Case True
                        StartAnimation()
                    Case False
                        StopAnimation()
                End Select
            End Set
        End Property
        Private _AnimationEnabled As Boolean = False

        Public ReadOnly Property isAnimating As Boolean
            Get
                If InternalAnimation Is Nothing Then
                    Return False
                Else
                    Return InternalAnimation.Running
                End If
            End Get
        End Property

        Public Overloads Property CurrentFrame As Integer
            Get
                Return _CurrentFrame
            End Get
            Set(value As Integer)
                If _CurrentFrame <> value Then
                    RaiseEvent TimelineFrameChanged(value)
                End If
                _CurrentFrame = value
            End Set
        End Property
        Private _CurrentFrame As Integer = 0

        Public Property AnimationCollections As New Dictionary(Of Byte, SpriteCollection)

        Public Property ActiveCollectionIndex As Byte
            Get
                Return _ActiveCollectionIndex
            End Get
            Set(value As Byte)
                If AnimationCollections.ContainsKey(value) Then
                    _ActiveCollectionIndex = value
                    ActiveCollection = AnimationCollections(_ActiveCollectionIndex)
                    CachedInterval = 1000 / _FPS
                    If InternalAnimation IsNot Nothing Then
                        InternalAnimation.To = ActiveCollection.Count - 1
                    End If
                Else
                    Throw New Exception(String.Format("Animation Collection does not contain the key '{0}'.", {value}))
                End If
            End Set
        End Property
        Private _ActiveCollectionIndex As Byte = 0

        Public Property ActiveCollection As SpriteCollection
            Get
                Return _ActiveCollection
            End Get
            Set(value As SpriteCollection)
                _ActiveCollection = value
                RaiseEvent ActiveCollectionChanged(_ActiveCollection)
            End Set
        End Property
        Private _ActiveCollection As SpriteCollection

#End Region

#Region "Animation Properties"
        Public AnimationProgressProperty As New SpriteProgressProperty(Me)

#Region "Animation Instances"
        Private WithEvents InternalAnimation As Animations.DoubleAnimation

        Public Sub StartAnimation()
            InitializeAnimation()
            InternalAnimation.Start()
        End Sub

        Public Sub StopAnimation()
            If InternalAnimation IsNot Nothing Then
                InternalAnimation.Stop()
            End If
        End Sub

        Private Sub InitializeAnimation()
            InternalAnimation = New Animations.DoubleAnimation( _
                   New CircleEase(EaseType.EaseInOut), _
                   0, ActiveCollection.Count - 1, _
                   TimeSpan.FromMilliseconds(CachedInterval), Scene.gameTime) With {.AutoRepeat = _AutoRepeat}
            BindAnimation(AnimationProgressProperty, InternalAnimation)
        End Sub

#End Region

#End Region

#Region "Events"
        Public Event TimelineFrameChanged(ByRef CurrentFrame As Integer)
        Public Event ActiveCollectionChanged(ByRef ActiveCollection As SpriteCollection)
#End Region

        Protected Friend Overrides Sub Draw(gameTime As Microsoft.Xna.Framework.GameTime)
            If ActiveCollection IsNot Nothing Then
                spriteBatch.Draw(ActiveCollection.Frame(CurrentFrame), Rectangle, Color.White)
            End If
        End Sub

        Public Overrides Sub Tick(gameTime As Microsoft.Xna.Framework.GameTime)
            MyBase.Tick(gameTime)
        End Sub

#Region "Constructors"
        Public Sub New(Scene As [Shared].Drawing.Scene)
            MyBase.New(Scene)
        End Sub

        Public Sub New(Scene As [Shared].Drawing.Scene, spriteBatch As SpriteBatch)
            MyBase.New(Scene, spriteBatch)
        End Sub
#End Region

        Private Sub SpriteElement_ActiveCollectionChanged(ByRef ActiveCollection As SpriteCollection) Handles Me.ActiveCollectionChanged
            If InternalAnimation IsNot Nothing Then
                InternalAnimation.To = ActiveCollection.Count - 1
            End If
        End Sub

    End Class

End Namespace
