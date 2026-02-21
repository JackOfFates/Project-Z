#If WINDOWS Then
Imports System.Runtime.InteropServices
Imports System.Diagnostics

Namespace Windows.Input

    ''' <summary>
    ''' Class for intercepting low level Windows mouse hooks.
    ''' </summary>
    Public Class MouseHook
        ''' <summary>
        ''' Internal callback processing function
        ''' </summary>
        Private Delegate Function MouseHookHandler(nCode As Integer, wParam As IntPtr, lParam As IntPtr) As IntPtr
        Private hookHandler As MouseHookHandler

        ''' <summary>
        ''' Function to be called when defined even occurs
        ''' </summary>
        ''' <param name="mouseStruct">MSLLHOOKSTRUCT mouse structure</param>
        Public Delegate Sub MouseHookCallback(mouseStruct As MSLLHOOKSTRUCT)

#Region "Events"
        Public Event LeftButtonDown As MouseHookCallback
        Public Event LeftButtonUp As MouseHookCallback
        Public Event RightButtonDown As MouseHookCallback
        Public Event RightButtonUp As MouseHookCallback
        Public Event MouseMove As MouseHookCallback
        Public Event MouseWheel As MouseHookCallback
        Public Event DoubleClick As MouseHookCallback
        Public Event MiddleButtonDown As MouseHookCallback
        Public Event MiddleButtonUp As MouseHookCallback
#End Region

        ''' <summary>
        ''' Low level mouse hook's ID
        ''' </summary>
        Private hookID As IntPtr = IntPtr.Zero

        ''' <summary>
        ''' Install low level mouse hook
        ''' </summary>
        Public Sub Install()
            hookHandler = AddressOf HookFunc
            hookID = SetHook(hookHandler)
        End Sub

        ''' <summary>
        ''' Remove low level mouse hook
        ''' </summary>
        Public Sub Uninstall()
            If hookID = IntPtr.Zero Then
                Return
            End If

            UnhookWindowsHookEx(hookID)
            hookID = IntPtr.Zero
        End Sub

        ''' <summary>
        ''' Destructor. Unhook current hook
        ''' </summary>
        Protected Overrides Sub Finalize()
            Try
                Uninstall()
            Finally
                MyBase.Finalize()
            End Try
        End Sub

        ''' <summary>
        ''' Sets hook and assigns its ID for tracking
        ''' </summary>
        ''' <param name="proc">Internal callback function</param>
        ''' <returns>Hook ID</returns>
        Private Function SetHook(proc As MouseHookHandler) As IntPtr
            Using [module] As ProcessModule = Process.GetCurrentProcess().MainModule
                Return SetWindowsHookEx(WH_MOUSE_LL, proc, GetModuleHandle([module].ModuleName), 0)
            End Using
        End Function

        ''' <summary>
        ''' Callback function
        ''' </summary>
        Private Function HookFunc(nCode As Integer, wParam As IntPtr, lParam As IntPtr) As IntPtr
            ' parse system messages
            If nCode >= 0 Then
                If MouseMessages.WM_LBUTTONDOWN = CType(wParam, MouseMessages) Then
                    RaiseEvent LeftButtonDown(CType(Marshal.PtrToStructure(lParam, GetType(MSLLHOOKSTRUCT)), MSLLHOOKSTRUCT))
                End If
                If MouseMessages.WM_LBUTTONUP = CType(wParam, MouseMessages) Then
                    RaiseEvent LeftButtonUp(CType(Marshal.PtrToStructure(lParam, GetType(MSLLHOOKSTRUCT)), MSLLHOOKSTRUCT))
                End If
                If MouseMessages.WM_RBUTTONDOWN = CType(wParam, MouseMessages) Then
                    RaiseEvent RightButtonDown(CType(Marshal.PtrToStructure(lParam, GetType(MSLLHOOKSTRUCT)), MSLLHOOKSTRUCT))
                End If
                If MouseMessages.WM_RBUTTONUP = CType(wParam, MouseMessages) Then
                    RaiseEvent RightButtonUp(CType(Marshal.PtrToStructure(lParam, GetType(MSLLHOOKSTRUCT)), MSLLHOOKSTRUCT))
                End If
                If MouseMessages.WM_MOUSEMOVE = CType(wParam, MouseMessages) Then
                    RaiseEvent MouseMove(CType(Marshal.PtrToStructure(lParam, GetType(MSLLHOOKSTRUCT)), MSLLHOOKSTRUCT))
                End If
                If MouseMessages.WM_MOUSEWHEEL = CType(wParam, MouseMessages) Then
                    RaiseEvent MouseWheel(CType(Marshal.PtrToStructure(lParam, GetType(MSLLHOOKSTRUCT)), MSLLHOOKSTRUCT))
                End If
                If MouseMessages.WM_LBUTTONDBLCLK = CType(wParam, MouseMessages) Then
                    RaiseEvent DoubleClick(CType(Marshal.PtrToStructure(lParam, GetType(MSLLHOOKSTRUCT)), MSLLHOOKSTRUCT))
                End If
                If MouseMessages.WM_MBUTTONDOWN = CType(wParam, MouseMessages) Then
                    RaiseEvent MiddleButtonDown(CType(Marshal.PtrToStructure(lParam, GetType(MSLLHOOKSTRUCT)), MSLLHOOKSTRUCT))
                End If
                If MouseMessages.WM_MBUTTONUP = CType(wParam, MouseMessages) Then
                    RaiseEvent MiddleButtonUp(CType(Marshal.PtrToStructure(lParam, GetType(MSLLHOOKSTRUCT)), MSLLHOOKSTRUCT))
                End If
            End If
            Return CallNextHookEx(hookID, nCode, wParam, lParam)
        End Function

#Region "WinAPI"
        Private Const WH_MOUSE_LL As Integer = 14

        Private Enum MouseMessages
            WM_LBUTTONDOWN = &H201
            WM_LBUTTONUP = &H202
            WM_MOUSEMOVE = &H200
            WM_MOUSEWHEEL = &H20A
            WM_RBUTTONDOWN = &H204
            WM_RBUTTONUP = &H205
            WM_LBUTTONDBLCLK = &H203
            WM_MBUTTONDOWN = &H207
            WM_MBUTTONUP = &H208
        End Enum

        <StructLayout(LayoutKind.Sequential)>
        Public Structure POINT
            Public x As Integer
            Public y As Integer
        End Structure

        <StructLayout(LayoutKind.Sequential)>
        Public Structure MSLLHOOKSTRUCT
            Public pt As POINT
            Public mouseData As UInteger
            Public flags As UInteger
            Public time As UInteger
            Public dwExtraInfo As IntPtr
        End Structure

        <DllImport("user32.dll", CharSet:=CharSet.Auto, SetLastError:=True)>
        Private Shared Function SetWindowsHookEx(idHook As Integer, lpfn As MouseHookHandler, hMod As IntPtr, dwThreadId As UInteger) As IntPtr
        End Function

        <DllImport("user32.dll", CharSet:=CharSet.Auto, SetLastError:=True)>
        Public Shared Function UnhookWindowsHookEx(hhk As IntPtr) As <MarshalAs(UnmanagedType.Bool)> Boolean
        End Function

        <DllImport("user32.dll", CharSet:=CharSet.Auto, SetLastError:=True)>
        Private Shared Function CallNextHookEx(hhk As IntPtr, nCode As Integer, wParam As IntPtr, lParam As IntPtr) As IntPtr
        End Function

        <DllImport("kernel32.dll", CharSet:=CharSet.Auto, SetLastError:=True)>
        Private Shared Function GetModuleHandle(lpModuleName As String) As IntPtr
        End Function
#End Region
    End Class
End Namespace

#End If