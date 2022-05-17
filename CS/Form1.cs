using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraGrid;
using DevExpress.XtraPivotGrid;

namespace ConditionsExample {

    public partial class Form1 : Form {
        const int PivotCount = 3;
        static string Field1Name = "fieldField1";
        static string Field2Name = "fieldField2";
        static Color conditionBackColor = Color.FromArgb(255, 128, 128);

        static string[] Expressions = new string[PivotCount] { 
            "[" + Field1Name + "] != [" + Field2Name + "]",
            "[" + Field1Name + "] > 0", "[" + Field1Name + "] > 0"};

        static string[] ConditionFieldNames = new string[PivotCount] { Field2Name, Field1Name, null };

        Label[] labels = new Label[PivotCount];
        PivotGridControl[] pivotGridControls = new PivotGridControl[PivotCount];

        public Form1() {
            InitializeComponent();
            Initialize();
        }

        void Initialize() {
            for (int i = 0; i < PivotCount; i++) {
                // pivots
                pivotGridControls[i] = new PivotGridControl();
                pivotGridControls[i].Location = new Point(12, 12 + i * 220);
                pivotGridControls[i].Size = new Size(750, 200);
                this.Controls.Add(pivotGridControls[i]);

                // labels
                labels[i] = new Label();
                labels[i].AutoSize = true;
                labels[i].Location = new Point(790, 107 + i * 220);
                labels[i].Size = new Size(35, 14);
                labels[i].Text = "label" + i;
                this.Controls.Add(labels[i]);

                InitializePivot(i, Expressions[i], ConditionFieldNames[i]);
            }
        }

        DataView GetDataView() {
            DataTable table = new DataTable();
            table.Columns.Add("Name", typeof(string));
            table.Columns.Add("Field1", typeof(int));
            table.Columns.Add("Field2", typeof(int));
            table.Columns.Add("Date", typeof(DateTime));
            table.Rows.Add("Name 1", 1, 1, DateTime.Today);
            table.Rows.Add("Name 1", 1, 2, DateTime.Today.AddDays(1));
            table.Rows.Add("Name 2", 0, 0, DateTime.Today);
            table.Rows.Add("Name 2", 5, 5, DateTime.Today.AddDays(1));
            return table.DefaultView;
        }

        void InitializePivot(int pivotIndex, string expression, string name) {
            PivotGridControl pivot = pivotGridControls[pivotIndex];
            
            pivot.BeginUpdate();
            pivot.OptionsData.DataProcessingEngine = PivotDataProcessingEngine.Optimized;
            pivot.Fields.Clear();
            pivot.Fields.Add(GetField("Name", "fieldName", PivotArea.RowArea));
            pivot.Fields.Add(GetField("Field1", Field1Name, PivotArea.DataArea));
            pivot.Fields.Add(GetField("Field2", Field2Name, PivotArea.DataArea));
            pivot.Fields.Add(GetField("Date", "fieldDate", PivotArea.ColumnArea));
            pivot.DataSource = GetDataView();
            pivot.EndUpdate();

            if (string.IsNullOrEmpty(name)) {
                SetCondition(pivot, expression, null);
                name = "all fields";
            }
            else
                SetCondition(pivot, expression, pivot.Fields[name]);
            this.labels[pivotIndex].Text = expression + "   =>  '" + name + "' cells";
        }

        PivotGridField GetField(string fieldName, string name, PivotArea area) {
            PivotGridField field = new PivotGridField();
            field.DataBinding = new DataSourceColumnBinding(fieldName);
            field.Area = area;
            field.Name = name;
            return field;
        }

        void SetCondition(PivotGridControl pivot, string expression, PivotGridField field) {
            PivotGridFormatConditionCollection conditions = pivot.FormatConditions;
            PivotGridStyleFormatCondition condition = new PivotGridStyleFormatCondition();
            condition.Appearance.BackColor = conditionBackColor;
            condition.Appearance.Options.UseBackColor = true;
            condition.Condition = FormatConditionEnum.Expression;
            condition.Expression = expression;
            condition.Field = field;
            conditions.Clear();
            conditions.Add(condition);
        }
    }
}