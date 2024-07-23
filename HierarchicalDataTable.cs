using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExpandableGridView;

namespace WindowsFormsApp1
{
    enum DataLevel : int
    {
        MainCycle = 0,
        SmallCycle=1,
        Step = 2,
        Record = 3
    }

    public enum ExpandStatus
    {
        NAN,
        Expand,
        Collapse
    }

    public class HierarchicalDataTable
    {
        private const string LEVELMARK = "LEVELMARK";               //记录datatable属于哪个级别，从0（root）开始
        private List<DataRow> _rows;
        private List<HierarchicalDataTable> _children;
        public bool HasChildNode => _children.Count > 0;
        public List<DataRow> Rows => _rows;
        private HierarchicalDataTable _parent = null;
        bool? _isExpanded = null; // null:没有子节点；false:有子节点但没展开;true:有子节点且已展开
        private readonly int SPACENUM = 3;

        //TODO:根据cycleIndex和stepindex找到对用子节点并添加到根节点rows中；

        public HierarchicalDataTable(DataTable srcDataTable)
        {
            _rows = srcDataTable.AsEnumerable().ToList();
            _children = new List<HierarchicalDataTable>();
            srcDataTable.ExtendedProperties[LEVELMARK] = 0;
        }

        private HierarchicalDataTable(DataTable srcDataTable, bool isSave) //子节点构造器，_rows为null，不记录数据，减少内存占用----->改为第1层子节点（Step）保留数据，最后一层不保留？
        {
            _rows = isSave ? srcDataTable.AsEnumerable().ToList() : null;
            _children = new List<HierarchicalDataTable>();
        }

        public void AddChild(DataTable childData, int targetRowIndex, bool isSave=false)
        {
            HierarchicalDataTable rootDt = GetRootDt();
            if (targetRowIndex < 0 || targetRowIndex >= rootDt._rows.Count)
                throw new IndexOutOfRangeException("根节点行数<目标行数");

            int level = Convert.ToInt32(rootDt.Rows[targetRowIndex].Table.ExtendedProperties[LEVELMARK]) + 1;
            childData.ExtendedProperties[LEVELMARK] = level;

            //创建子节点，设置其父节点
            var child = new HierarchicalDataTable(childData, isSave);
            child._parent = this;
            child._parent._isExpanded = true;
            foreach (DataRow row in childData.Rows)
            {
                row[0] = "".PadLeft(level * SPACENUM, ' ') + row[0].ToString();
            }

            //在根节点添加数据
            rootDt._rows.InsertRange(targetRowIndex + 1, childData.AsEnumerable().ToList());
            
            rootDt._rows[targetRowIndex][0] = "[-]" + rootDt._rows[targetRowIndex][0].ToString().Replace("[+]", ""); //根节点对应行添加展开图标

            _children.Add(child);
        }

        public void SetExpandStatus(int rowIndex, ExpandStatus state)
        {
            var rootDt = GetRootDt();
            if (rowIndex >= rootDt._rows.Count)
                throw new IndexOutOfRangeException("根节点行数不满足");
            //获取当前rowIndex对应的datatable,得到level
            int level = Convert.ToInt32(rootDt.Rows[rowIndex].Table.ExtendedProperties[LEVELMARK]);

            switch (state)
            {
                case ExpandStatus.NAN:
                    break;
                case ExpandStatus.Expand:
                    var rowStr = rootDt.Rows[rowIndex][0].ToString().Replace("[-]", "");
                    rootDt.Rows[rowIndex][0] = $"[+]{rootDt.Rows[rowIndex][0]}";
                    break;
                case ExpandStatus.Collapse:
                    break;
                default:
                    break;
            }
        }

        private HierarchicalDataTable GetRootDt()
        {
            HierarchicalDataTable rootDt = this;
            while (rootDt._parent != null)
            {
                rootDt = this._parent;
            }
            return rootDt;
        }

        //private int GetLevel(int rowIndex)
        //{
        //    var rootDt = GetRootDt();
        //    return Convert.ToInt32(rootDt.Rows[rowIndex].Table.TableName);
        //}
    }
}
