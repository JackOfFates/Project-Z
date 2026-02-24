Imports Microsoft.Xna.Framework
Imports Microsoft.Xna.Framework.Graphics
Imports ProjectZ.Shared.Drawing
Imports ProjectZ.Shared.Drawing.UI.Primitives
Imports ProjectZ.Shared.Drawing.UI.Advanced

Namespace TowerDefense
    Public Module EnemyColorRamp
        Private ReadOnly Ramp As Color() = {Color.Gray, Color.Red, Color.Blue, Color.Yellow, Color.Purple}

        Public Function GetColor(blend As Single) As Color
            Dim clamped As Single = Math.Min(1.0F, Math.Max(0.0F, blend))
            Dim segmentLength As Single = 1.0F / (Ramp.Length - 1)
            Dim index As Integer = Math.Min(Ramp.Length - 2, CInt(Math.Floor(clamped / segmentLength)))
            Dim localBlend As Single = (clamped - index * segmentLength) / segmentLength
            Return Color.Lerp(Ramp(index), Ramp(index + 1), localBlend)
        End Function
    End Module

    Public Class CircleCollider
        Public Property Position As Vector2
        Public Property Radius As Single

        Public Function Intersects(other As CircleCollider) As Boolean
            Dim distance As Single = Vector2.DistanceSquared(Position, other.Position)
            Dim minDistance As Single = Radius + other.Radius
            Return distance <= minDistance * minDistance
        End Function
    End Class

    Public Class RigidBody2D
        Public Property Velocity As Vector2
        Public Property Damping As Single = 0.98F

        Public Function Integrate(position As Vector2, deltaSeconds As Single) As Vector2
            Dim newPosition As Vector2 = position + Velocity * deltaSeconds
            Velocity *= Damping
            Return newPosition
        End Function
    End Class

    Public Class Enemy
        Public Property Id As Guid = Guid.NewGuid()
        Public Property Visual As CircleElement
        Public Property Position As Vector2
        Public Property Radius As Single
        Public Property Speed As Single
        Public Property Health As Single
        Public Property MaxHealth As Single
        Public Property TypeBlend As Single
        Public Property PathIndex As Integer
        Public Property ReachedEnd As Boolean

        Public Sub New(scene As Scene, position As Vector2, radius As Single, speed As Single, maxHealth As Single, typeBlend As Single)
            Me.Position = position
            Me.Radius = radius
            Me.Speed = speed
            Me.MaxHealth = maxHealth
            Me.Health = maxHealth
            Me.TypeBlend = typeBlend
            Visual = New CircleElement(scene) With {
                .Size = New Vector2(radius * 2.0F),
                .FillColor = EnemyColorRamp.GetColor(typeBlend)
            }
            UpdateVisual()
        End Sub

        Public Function Collider() As CircleCollider
            Return New CircleCollider With {.Position = Position, .Radius = Radius}
        End Function

        Public Sub Update(path As List(Of Vector2), deltaSeconds As Single)
            If ReachedEnd OrElse path Is Nothing OrElse path.Count < 2 Then Return
            Dim nextIndex As Integer = Math.Min(PathIndex + 1, path.Count - 1)
            Dim target As Vector2 = path(nextIndex)
            Dim direction As Vector2 = target - Position
            Dim distance As Single = direction.Length()
            If distance <= 0.1F Then
                PathIndex = nextIndex
                If PathIndex >= path.Count - 1 Then
                    ReachedEnd = True
                End If
            Else
                direction.Normalize()
                Dim stepAmount As Single = Speed * deltaSeconds
                If stepAmount >= distance Then
                    Position = target
                    PathIndex = nextIndex
                    If PathIndex >= path.Count - 1 Then
                        ReachedEnd = True
                    End If
                Else
                    Position += direction * stepAmount
                End If
            End If
            UpdateVisual()
        End Sub

        Public Sub UpdateVisual()
            Visual.Position = Position - New Vector2(Radius)
            Visual.FillColor = EnemyColorRamp.GetColor(TypeBlend)
        End Sub

        Public Function IsDead() As Boolean
            Return Health <= 0
        End Function
    End Class

    Public Class Bomb
        Public Property Id As Guid = Guid.NewGuid()
        Public Property Visual As CircleElement
        Public Property TimerText As TextElement
        Public Property Position As Vector2
        Public Property Radius As Single
        Public Property Countdown As Single
        Public Property ExplosionRadius As Single

        Public Sub New(scene As Scene, position As Vector2, radius As Single, countdown As Single, explosionRadius As Single)
            Me.Position = position
            Me.Radius = radius
            Me.Countdown = countdown
            Me.ExplosionRadius = explosionRadius
            Visual = New CircleElement(scene) With {
                .Size = New Vector2(radius * 2.0F),
                .FillColor = Color.Orange
            }
            TimerText = New TextElement(scene) With {
                .ForegroundColor = Color.White,
                .Text = Math.Ceiling(countdown).ToString()
            }
            UpdateVisual()
        End Sub

        Public Sub Update(deltaSeconds As Single)
            Countdown = Math.Max(0.0F, Countdown - deltaSeconds)
            TimerText.Text = Math.Ceiling(Countdown).ToString()
            UpdateVisual()
        End Sub

        Public Sub UpdateVisual()
            Visual.Position = Position - New Vector2(Radius)
            TimerText.Position = Position - New Vector2(TimerText.Size.X / 2.0F, TimerText.Size.Y / 2.0F)
        End Sub

        Public Function Collider() As CircleCollider
            Return New CircleCollider With {.Position = Position, .Radius = Radius}
        End Function
    End Class

    Public Class HealthOrb
        Public Property Id As Guid = Guid.NewGuid()
        Public Property Visual As CircleElement
        Public Property Position As Vector2
        Public Property Radius As Single
        Public Property Amount As Single

        Public Sub New(scene As Scene, position As Vector2, radius As Single, amount As Single)
            Me.Position = position
            Me.Radius = radius
            Me.Amount = amount
            Visual = New CircleElement(scene) With {
                .Size = New Vector2(radius * 2.0F),
                .FillColor = Color.Green
            }
            UpdateVisual()
        End Sub

        Public Sub UpdateVisual()
            Visual.Position = Position - New Vector2(Radius)
        End Sub

        Public Function Collider() As CircleCollider
            Return New CircleCollider With {.Position = Position, .Radius = Radius}
        End Function
    End Class

    Public Class Projectile
        Public Property Id As Guid = Guid.NewGuid()
        Public Property Visual As PolygonElement
        Public Property Position As Vector2
        Public Property Radius As Single
        Public Property RigidBody As RigidBody2D
        Public Property Damage As Single
        Public Property Lifetime As Single

        Public Sub New(scene As Scene, position As Vector2, radius As Single, velocity As Vector2, damage As Single, damping As Single)
            Me.Position = position
            Me.Radius = radius
            Me.Damage = damage
            RigidBody = New RigidBody2D With {.Velocity = velocity, .Damping = damping}
            Visual = New PolygonElement(scene) With {
                .FillColor = Color.LightBlue
            }
            UpdateVisual()
        End Sub

        Public Sub Update(deltaSeconds As Single)
            Position = RigidBody.Integrate(Position, deltaSeconds)
            Lifetime += deltaSeconds
            UpdateVisual()
        End Sub

        Public Sub UpdateVisual()
            Dim direction As Vector2 = RigidBody.Velocity
            If direction.LengthSquared() < 0.001F Then
                direction = New Vector2(1.0F, 0.0F)
            Else
                direction.Normalize()
            End If
            Dim length As Single = 18.0F
            Dim thickness As Single = 4.0F
            Dim perp As New Vector2(-direction.Y, direction.X)
            Dim halfLength As Vector2 = direction * (length / 2.0F)
            Dim halfWidth As Vector2 = perp * (thickness / 2.0F)
            Dim p1 As Vector2 = Position - halfLength - halfWidth
            Dim p2 As Vector2 = Position - halfLength + halfWidth
            Dim p3 As Vector2 = Position + halfLength + halfWidth
            Dim p4 As Vector2 = Position + halfLength - halfWidth
            Visual.Position = Vector2.Zero
            Visual.Vectors = {p1, p2, p3, p4}
            Visual.ApplyGeometryChanges()
            Dim pulse As Single = CSng(0.5F + 0.5F * Math.Sin(Lifetime * 8.0F))
            Visual.FillColor = Color.Lerp(Color.DeepSkyBlue, Color.White, pulse)
        End Sub

        Public Function Collider() As CircleCollider
            Return New CircleCollider With {.Position = Position, .Radius = Radius}
        End Function
    End Class

    Public Class RippleEffect
        Public Property Position As Vector2
        Public Property MaxRadius As Single
        Public Property Lifetime As Single
        Public Property MaxLifetime As Single
        Public Property Visuals As List(Of CircleElement)
        Private ReadOnly NumRings As Integer = 3
        Private ReadOnly RingDelay As Single = 0.08F
        Private ReadOnly RingThickness As Single = 4.0F

        Public Sub New(scene As Scene, position As Vector2, Optional maxRadius As Single = 60.0F, Optional duration As Single = 0.5F)
            Me.Position = position
            Me.MaxRadius = maxRadius
            Me.Lifetime = 0.0F
            Me.MaxLifetime = duration
            Me.Visuals = New List(Of CircleElement)

            For i As Integer = 0 To NumRings - 1
                Dim ring As New CircleElement(scene) With {
                    .FillColor = New Color(255, 255, 255, 80),
                    .Size = Vector2.Zero,
                    .isMouseBypassEnabled = True
                }
                Visuals.Add(ring)
            Next
        End Sub

        Public Function Update(deltaSeconds As Single) As Boolean
            Lifetime += deltaSeconds
            If Lifetime >= MaxLifetime + (NumRings - 1) * RingDelay Then
                Return True ' Effect complete
            End If

            For i As Integer = 0 To Visuals.Count - 1
                Dim ringTime As Single = Lifetime - (i * RingDelay)
                If ringTime < 0 Then
                    Visuals(i).Size = Vector2.Zero
                    Continue For
                End If

                Dim progress As Single = Math.Min(1.0F, ringTime / MaxLifetime)
                Dim easeOut As Single = 1.0F - (1.0F - progress) * (1.0F - progress)
                Dim radius As Single = MaxRadius * easeOut
                Dim alpha As Integer = CInt((1.0F - progress) * 120.0F)

                Visuals(i).Size = New Vector2(radius * 2.0F)
                Visuals(i).Position = Position - New Vector2(radius)
                Visuals(i).FillColor = New Color(255, 255, 255, Math.Max(0, alpha))
            Next

            Return False
        End Function
    End Class

    Public Enum TargetingMode
        First
        Last
        Closest
        Strongest
        Weakest
    End Enum

    Public MustInherit Class Tower
        Public Property Id As Guid = Guid.NewGuid()
        Public Property Position As Vector2
        Public Property Range As Single
        Public Property FireCooldown As Single
        Public Property FireRate As Single
        Public Property Damage As Single
        Public Property Visual As CircleElement
        Public Property Cost As Integer
        Public Property Level As Integer = 1
        Public Property MaxLevel As Integer = 5
        Public Property UpgradeCost As Integer
        Public Property Targeting As TargetingMode = TargetingMode.First
        Public Property TotalInvested As Integer = 0

        Public Sub New(scene As Scene, position As Vector2, radius As Single, range As Single, fireRate As Single, damage As Single, cost As Integer, upgradeCost As Integer, color As Color)
            Me.Position = position
            Me.Range = range
            Me.FireRate = fireRate
            Me.Damage = damage
            Me.Cost = cost
            Me.TotalInvested = cost
            Me.UpgradeCost = upgradeCost
            Visual = New CircleElement(scene) With {
                .Size = New Vector2(radius * 2.0F),
                .FillColor = color
            }
            UpdateVisual()
        End Sub

        Public Sub Update(deltaSeconds As Single, enemies As List(Of Enemy), projectiles As List(Of Projectile), scene As Scene)
            FireCooldown = Math.Max(0.0F, FireCooldown - deltaSeconds)
            If FireCooldown > 0 Then Return
            Dim target As Enemy = SelectTarget(enemies)
            If target Is Nothing Then Return
            FireCooldown = 1.0F / FireRate
            Fire(target, projectiles, scene)
        End Sub

        Public Sub CycleTargeting()
            Dim modes = [Enum].GetValues(GetType(TargetingMode))
            Dim currentIndex As Integer = Array.IndexOf(modes, Targeting)
            Dim nextIndex As Integer = (currentIndex + 1) Mod modes.Length
            Targeting = CType(modes.GetValue(nextIndex), TargetingMode)
        End Sub

        Protected Overridable Function SelectTarget(enemies As List(Of Enemy)) As Enemy
            Dim inRange As New List(Of Enemy)
            Dim rangeSquared As Single = Range * Range

            For Each enemy As Enemy In enemies
                If enemy Is Nothing OrElse enemy.IsDead() Then Continue For
                Dim distance As Single = Vector2.DistanceSquared(enemy.Position, Position)
                If distance <= rangeSquared Then
                    inRange.Add(enemy)
                End If
            Next

            If inRange.Count = 0 Then Return Nothing

            Select Case Targeting
                Case TargetingMode.First
                    ' Enemy furthest along the path (highest PathIndex)
                    Return inRange.OrderByDescending(Function(e) e.PathIndex).ThenByDescending(Function(e) e.Speed).FirstOrDefault()
                Case TargetingMode.Last
                    ' Enemy least far along the path (lowest PathIndex)
                    Return inRange.OrderBy(Function(e) e.PathIndex).ThenBy(Function(e) e.Speed).FirstOrDefault()
                Case TargetingMode.Closest
                    ' Closest enemy to tower
                    Return inRange.OrderBy(Function(e) Vector2.DistanceSquared(e.Position, Position)).FirstOrDefault()
                Case TargetingMode.Strongest
                    ' Enemy with most health
                    Return inRange.OrderByDescending(Function(e) e.Health).FirstOrDefault()
                Case TargetingMode.Weakest
                    ' Enemy with least health
                    Return inRange.OrderBy(Function(e) e.Health).FirstOrDefault()
                Case Else
                    Return inRange.FirstOrDefault()
            End Select
        End Function

        Protected MustOverride Sub Fire(target As Enemy, projectiles As List(Of Projectile), scene As Scene)

        Public Sub UpdateVisual()
            Visual.Position = Position - New Vector2(Visual.Size.X / 2.0F)
        End Sub

        Public Function CanUpgrade() As Boolean
            Return Level < MaxLevel
        End Function

        Public Function GetSellValue() As Integer
            Return CInt(Math.Floor(TotalInvested * 2.0 / 3.0))
        End Function

        Public Function Upgrade(ByRef cost As Integer) As Boolean
            If Not CanUpgrade() Then Return False
            cost = UpgradeCost
            TotalInvested += UpgradeCost
            Level += 1
            Range *= 1.06F
            FireRate *= 1.08F
            Damage *= 1.12F
            UpgradeCost = CInt(Math.Ceiling(UpgradeCost * 1.5F))
            Return True
        End Function
    End Class

    Public Class PointTower
        Inherits Tower

        Public Sub New(scene As Scene, position As Vector2)
            MyBase.New(scene, position, 18.0F, 180.0F, 1.2F, 8.0F, 50, 35, Color.SlateGray)
        End Sub

        Protected Overrides Sub Fire(target As Enemy, projectiles As List(Of Projectile), scene As Scene)
            Dim direction As Vector2 = target.Position - Position
            If direction.LengthSquared() < 0.1F Then Return
            direction.Normalize()
            Dim projectile As New Projectile(scene, Position, 6.0F, direction * 450.0F, Damage, 1.0F)
            projectiles.Add(projectile)
        End Sub
    End Class

    Public Class SpiralTower
        Inherits Tower

        Private angle As Single

        Public Sub New(scene As Scene, position As Vector2)
            MyBase.New(scene, position, 18.0F, 200.0F, 1.6F, 6.0F, 65, 45, Color.DarkOrange)
        End Sub

        Protected Overrides Sub Fire(target As Enemy, projectiles As List(Of Projectile), scene As Scene)
            angle += 0.6F
            Dim direction As New Vector2(CSng(Math.Cos(angle)), CSng(Math.Sin(angle)))
            Dim projectile As New Projectile(scene, Position, 5.0F, direction * 380.0F, Damage, 1.0F)
            projectiles.Add(projectile)
        End Sub
    End Class

    Public Class BurstTower
        Inherits Tower

        Public Sub New(scene As Scene, position As Vector2)
            MyBase.New(scene, position, 18.0F, 160.0F, 0.9F, 5.0F, 75, 55, Color.MediumPurple)
        End Sub

        Protected Overrides Sub Fire(target As Enemy, projectiles As List(Of Projectile), scene As Scene)
            Dim baseDirection As Vector2 = target.Position - Position
            If baseDirection.LengthSquared() < 0.1F Then Return
            baseDirection.Normalize()
            Dim angles As Single() = {-0.2F, 0.0F, 0.2F}
            For Each offset As Single In angles
                Dim rotated As Vector2 = New Vector2(
                    baseDirection.X * CSng(Math.Cos(offset)) - baseDirection.Y * CSng(Math.Sin(offset)),
                    baseDirection.X * CSng(Math.Sin(offset)) + baseDirection.Y * CSng(Math.Cos(offset)))
                Dim projectile As New Projectile(scene, Position, 5.0F, rotated * 420.0F, Damage, 1.0F)
                projectiles.Add(projectile)
            Next
        End Sub
    End Class

    Public Class RapidTower
        Inherits Tower

        Public Sub New(scene As Scene, position As Vector2)
            MyBase.New(scene, position, 16.0F, 150.0F, 2.4F, 3.5F, 40, 25, Color.LightSkyBlue)
        End Sub

        Protected Overrides Sub Fire(target As Enemy, projectiles As List(Of Projectile), scene As Scene)
            Dim direction As Vector2 = target.Position - Position
            If direction.LengthSquared() < 0.1F Then Return
            direction.Normalize()
            Dim projectile As New Projectile(scene, Position, 4.0F, direction * 520.0F, Damage, 1.0F)
            projectiles.Add(projectile)
        End Sub
    End Class

    Public Class SniperTower
        Inherits Tower

        Public Sub New(scene As Scene, position As Vector2)
            MyBase.New(scene, position, 18.0F, 260.0F, 0.6F, 14.0F, 90, 70, Color.DarkSeaGreen)
        End Sub

        Protected Overrides Sub Fire(target As Enemy, projectiles As List(Of Projectile), scene As Scene)
            Dim direction As Vector2 = target.Position - Position
            If direction.LengthSquared() < 0.1F Then Return
            direction.Normalize()
            Dim projectile As New Projectile(scene, Position, 6.0F, direction * 700.0F, Damage, 1.0F)
            projectiles.Add(projectile)
        End Sub
    End Class
End Namespace
