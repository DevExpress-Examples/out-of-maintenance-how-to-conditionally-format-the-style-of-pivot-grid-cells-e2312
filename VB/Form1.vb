Imports Microsoft.VisualBasic
Imports System
Imports System.Data
Imports System.Drawing
Imports System.Windows.Forms
Imports DevExpress.XtraGrid
Imports DevExpress.XtraPivotGrid

Namespace ConditionsExample

	Partial Public Class Form1
		Inherits Form
		Private Const PivotCount As Integer = 3
		Private Shared Field1Name As String = "fieldField1"
		Private Shared Field2Name As String = "fieldField2"
		Private Shared conditionBackColor As Color = Color.FromArgb(255, 128, 128)

		Private Shared Expressions() As String = { "[" & Field1Name & "] != [" & Field2Name & "]", "[" & Field1Name & "] > 0", "[" & Field1Name & "] > 0"}

		Private Shared ConditionFieldNames() As String = { Field2Name, Field1Name, Nothing }

		Private labels(PivotCount - 1) As Label
		Private pivotGridControls(PivotCount - 1) As PivotGridControl

		Public Sub New()
			InitializeComponent()
			Initialize()
		End Sub

		Private Sub Initialize()
			For i As Integer = 0 To PivotCount - 1
				' pivots
				pivotGridControls(i) = New PivotGridControl()
				pivotGridControls(i).Location = New Point(12, 12 + i * 220)
				pivotGridControls(i).Size = New Size(750, 200)
				Me.Controls.Add(pivotGridControls(i))

				' labels
				labels(i) = New Label()
				labels(i).AutoSize = True
				labels(i).Location = New Point(790, 107 + i * 220)
				labels(i).Size = New Size(35, 14)
				labels(i).Text = "label" & i
				Me.Controls.Add(labels(i))

				InitializePivot(i, Expressions(i), ConditionFieldNames(i))
			Next i
		End Sub

		Private Function GetDataView() As DataView
			Dim table As New DataTable()
			table.Columns.Add("Name", GetType(String))
			table.Columns.Add("Field1", GetType(Integer))
			table.Columns.Add("Field2", GetType(Integer))
			table.Columns.Add("Date", GetType(DateTime))
			table.Rows.Add("Name 1", 1, 1, DateTime.Today)
			table.Rows.Add("Name 1", 1, 2, DateTime.Today.AddDays(1))
			table.Rows.Add("Name 2", 0, 0, DateTime.Today)
			table.Rows.Add("Name 2", 5, 5, DateTime.Today.AddDays(1))
			Return table.DefaultView
		End Function

		Private Sub InitializePivot(ByVal pivotIndex As Integer, ByVal expression As String, ByVal name As String)
			Dim pivot As PivotGridControl = pivotGridControls(pivotIndex)

			pivot.BeginUpdate()
			pivot.OptionsData.DataProcessingEngine = PivotDataProcessingEngine.Optimized
			pivot.Fields.Clear()
			pivot.Fields.Add(GetField("Name", "fieldName", PivotArea.RowArea))
			pivot.Fields.Add(GetField("Field1", Field1Name, PivotArea.DataArea))
			pivot.Fields.Add(GetField("Field2", Field2Name, PivotArea.DataArea))
			pivot.Fields.Add(GetField("Date", "fieldDate", PivotArea.ColumnArea))
			pivot.DataSource = GetDataView()
			pivot.EndUpdate()

			If String.IsNullOrEmpty(name) Then
				SetCondition(pivot, expression, Nothing)
				name = "all fields"
			Else
				SetCondition(pivot, expression, pivot.Fields(name))
			End If
			Me.labels(pivotIndex).Text = expression & "   =>  '" & name & "' cells"
		End Sub

		Private Function GetField(ByVal fieldName As String, ByVal name As String, ByVal area As PivotArea) As PivotGridField
			Dim field As New PivotGridField()
			field.DataBinding = New DataSourceColumnBinding(fieldName)
			field.Area = area
			field.Name = name
			Return field
		End Function

		Private Sub SetCondition(ByVal pivot As PivotGridControl, ByVal expression As String, ByVal field As PivotGridField)
			Dim conditions As PivotGridFormatConditionCollection = pivot.FormatConditions
			Dim condition As New PivotGridStyleFormatCondition()
			condition.Appearance.BackColor = conditionBackColor
			condition.Appearance.Options.UseBackColor = True
			condition.Condition = FormatConditionEnum.Expression
			condition.Expression = expression
			condition.Field = field
			conditions.Clear()
			conditions.Add(condition)
		End Sub
	End Class
End Namespace