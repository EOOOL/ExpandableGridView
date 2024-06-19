using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using WindowsFormsApp1;

namespace ExpandableGridView
{
    
    public partial class Form1 : Form
    {
        private DataTable dataTable1;
        int getNum = 0;
        List<DataRow> storeDisplayedRow = new List<DataRow>();
        HierarchicalDataTable rootDt = null;

        public Form1()
        {
            InitializeComponent();
            InitializeGridView();
        }

        private void InitializeGridView()
        {
            dataTable1 = new DataTable();
            dataGridView1.AutoGenerateColumns = false;
            typeof(Control)
                .GetProperty("DoubleBuffered", BindingFlags.NonPublic | BindingFlags.Instance)
                .SetValue(dataGridView1, true, null);
            var cycleData = GetDataTable(60);
            rootDt = new HierarchicalDataTable(cycleData);
            var tmp1 = GetDataTable(3);
            var tmp2 = GetDataTable(4);
            rootDt.AddChild(tmp1, 2);
            rootDt.AddChild(tmp2, 3);
            for (int i = 0; i < cycleData.Rows.Count; i++)
            {
                //cycleData.Rows[i][0] = $"[+]{cycleData.Rows[i][0]}";
                DataTable cycleData1 = new DataTable();
                if (i % 2 == 0)
                    cycleData1 = GetDataTable(6);
                //主循环n有数据：
                if (cycleData1 != null && cycleData1.Rows.Count > 0)
                {
                    cycleData.Rows[i][0] = $"[+]{cycleData.Rows[i][0]}";
                }
                else {
                    cycleData.Rows[i][0] = $"   {cycleData.Rows[i][0]}";
                }
            }

            dataGridView1.RowHeadersVisible = false;
            dataGridView1.ReadOnly = true;
            dataGridView1.VirtualMode = true;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.CellValueNeeded += DataGridView1_CellValueNeeded;
            dataGridView1.RowCount = rootDt.Rows.Count;
        }

        private void DataGridView1_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                if (rootDt.Rows.Count <= e.RowIndex || rootDt.Rows[e.RowIndex].Table.Columns.Count <= e.ColumnIndex)
                    e.Value = string.Empty;
                else
                    e.Value = rootDt.Rows[e.RowIndex][e.ColumnIndex].ToString();

            }
        }

        private DataTable GetDataTable(int count)
        {
            DataTable dt = new DataTable();
            for (int i = 0; i < 35 + getNum; i++)
            {
                dt.Columns.Add($"Name{i}", typeof(string));
            }
            if (dataGridView1.Columns.Count < dt.Columns.Count)
            {
                for (int i = 0, k = dt.Columns.Count - dataGridView1.Columns.Count; i < k; i++)
                {
                    dataGridView1.Columns.Add(
                        new DataGridViewTextBoxColumn
                        {
                            HeaderText = dt.Columns[i].ColumnName,
                            Name = dt.Columns[i].ColumnName
                        }
                    );
                }
            }
            for (int i = 0; i < count; i++)
            {
                dt.Rows.Add(
                    $"{getNum}---{i}",
                    $"Jane{i}",
                    "John",
                    "John",
                    "John",
                    "John",
                    "John",
                    "John",
                    "John",
                    "John",
                    "John",
                    "John",
                    "John",
                    "John",
                    "John",
                    "John",
                    "John",
                    "John",
                    "John",
                    "John",
                    "John",
                    "John",
                    "John",
                    "John",
                    "John",
                    "John",
                    "John",
                    "John",
                    "John",
                    "John",
                    "John"
                );
            }
            getNum++;
            return dt;
        }

        private void dataGridView1_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.ColumnIndex != 0 || e.RowIndex < 0)
                return;
            DataGridView dataGridView = sender as DataGridView;
            string cellText =
                dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value?.ToString() ?? "";

            int plusPos = cellText.IndexOf("[+]");
            if (plusPos != -1)
            {
                // 获取"[+]"字符串的大小（考虑字体）
                Font cellFont = dataGridView1.DefaultCellStyle.Font;
                SizeF plusTextSize = TextRenderer.MeasureText("[+]", cellFont);

                // 计算"[+]"在单元格中的实际位置（X坐标）
                var cellRect = dataGridView1.GetCellDisplayRectangle(
                    e.ColumnIndex,
                    e.RowIndex,
                    false
                );
                cellRect.Offset(-2, 0);
                int plusPosX =
                    cellRect.X + TextRenderer.MeasureText(new string(' ', plusPos), cellFont).Width; // 考虑到空格的宽度

                // 判断点击是否落在"[+]"上
                bool isPlusClicked =
                    e.X >= plusPosX && e.X <= plusPosX + (int)plusTextSize.Width - 10;  // TextRenderer.MeasureText 方法返回的宽度可能包含额外的边距或间距，因此与单纯的字符宽度存在一定差异（GPT）
                                                                                        // 27寸显示器下测试，-10可以将范围限制在[+]内

                if (isPlusClicked) 
                {
                    //查询数据并增加到树形结构中
                    var tmp = GetDataTable(8);

                
                }
            }

            int minusPos = cellText.IndexOf("[-]");
            if (minusPos != -1)
            {
                Font cellFont = dataGridView1.DefaultCellStyle.Font;
                SizeF plusTextSize = TextRenderer.MeasureText("[-]", cellFont);

                var cellRect = dataGridView1.GetCellDisplayRectangle(
                    e.ColumnIndex,
                    e.RowIndex,
                    false
                );
                int plusPosX =
                    cellRect.X
                    + TextRenderer.MeasureText(new string(' ', minusPos), cellFont).Width;

                bool isPlusClicked =
                    e.X >= plusPosX && e.X <= plusPosX + (int)plusTextSize.Width - 10;

                if (isPlusClicked)
                {
                    MessageBox.Show("Clicked on '-'!");
                }
            }
        }
    }
}
