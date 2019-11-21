Imports NeuralProject

Public Class Form1

    Dim nv As NeuralVisualizer
    Dim nn As New NeuralNetwork(2, 4, 1)

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        nv = New NeuralVisualizer(pbox, nn)

        For I = 1 To 3
            nv.DrawLayer(I)
        Next
    End Sub

    Private Sub pbox_SizeChanged(sender As Object, e As EventArgs) Handles pbox.SizeChanged
        If nv IsNot Nothing Then
            nv.ResizeRenderBox(pbox)

            For I = 1 To 3
                nv.DrawLayer(I)
            Next
        End If
    End Sub

    Private Sub pbox_Click(sender As Object, e As EventArgs) Handles pbox.Click

    End Sub

End Class
