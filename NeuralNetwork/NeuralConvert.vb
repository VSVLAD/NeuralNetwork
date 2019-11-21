Option Strict On
Option Explicit On

Namespace NeuralProject

    ''' <summary>Класс предоставляет методы для подготовки и преобразования данных для нейросетей</summary>
    Public Class NeuralConvert

        ''' <summary>
        ''' Масштабирует числа из одного диапазона в другой.
        ''' Например, диапазон [-255, 255] можно преобразовать в [0, 1]
        ''' </summary>
        ''' <param name="Value">Выбранное число для масштабирования</param>
        ''' <param name="FromLow">Нижняя граница диапазона исходых чисел</param>
        ''' <param name="FromUp">Верхняя граница диапазона исходных чисел</param>
        ''' <param name="ToLow">Нижняя граница диапазона для результирующих чисел</param>
        ''' <param name="ToUp">Верхняя граница диапазона для результирующих чисел</param>
        ''' <returns>Преобразованное число</returns>
        Public Shared Function Scaler(Value As Double, FromLow As Double, FromUp As Double, ToLow As Double, ToUp As Double) As Double
            Return (Value - FromLow) * (ToUp - ToLow) / (FromUp - FromLow) + ToLow
        End Function

        ''' <summary>
        ''' Конвертирует дату из эпохи Unix в формат даты .NET
        ''' </summary>
        ''' <param name="UnixTime">Количество секунд</param>
        ''' <returns>Дата и время</returns>
        Public Shared Function FromUnixTime(UnixTime As Long) As Date
            Dim epoch = New Date(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            Return epoch.AddSeconds(UnixTime / 1000)
        End Function

        ''' <summary>
        ''' Конвертирует дату и время .NET в формат эпохи Unix с милисекундами. Число секунд после 1970-01-01
        ''' </summary>
        ''' <param name="DateValue">Дата</param>
        ''' <returns>Количество секунд</returns>
        Public Shared Function ToUnixTime(DateValue As Date) As Long
            Dim epoch = New DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            Return Convert.ToInt64((DateValue - epoch).TotalSeconds * 1000)
        End Function

    End Class

End Namespace