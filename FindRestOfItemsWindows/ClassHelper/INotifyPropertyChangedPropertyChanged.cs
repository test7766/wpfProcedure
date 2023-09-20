using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace Optimapharm.PSC.PurchasingManager.Windows.FindRestOfItemsWindows.ClassHelper
{
    public class INotifyPropertyChangedPropertyChanged : INotifyPropertyChanged, IEnumerable<KeyValuePair<string, string>>
    {

        public string _document_Number { get; set; }
        public string _doc_Type { get; set; }
        public string _doc_Co { get; set; }
        public string _transaction_Date { get; set; }
        public string _branch_Plant { get; set; }
        public string _quantity { get; set; }
        public string _trans_UoM { get; set; }
        public string _unit_Cost { get; set; }
        public string _extended_Cost { get; set; }
        public string _lot_Serial { get; set; }
        public string _location { get; set; }
        public string _lot_Status_Code { get; set; }
        public string _order_Number { get; set; }
        public string _order_Ty { get; set; }
        public string _oprder_Co { get; set; }
        public string _lineNum { get; set; }
        public string _class_Code { get; set; }
        public string _gL_Date { get; set; }
        public string _supplier_Lot_Number { get; set; }
        public string _trex { get; set; }
        public string _fT { get; set; }


        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        public string DocumentNumber
        {
            get { return _document_Number; }
            set
            {
                if (_document_Number != value)
                {
                    _document_Number = value;
                    OnPropertyChanged(nameof(DocumentNumber));
                }
            }
        }

        #region Field
        public string DocType
        {
            get { return _doc_Type; }
            set
            {
                if (_doc_Type != value)
                {
                    _doc_Type = value;
                    OnPropertyChanged(nameof(DocType));
                }
            }
        }
        public string DocCo
        {
            get { return _doc_Co; }
            set
            {
                if (_doc_Co != value)
                {
                    _doc_Co = value;
                    OnPropertyChanged(nameof(DocCo));
                }
            }
        }

        public string TransactionDate
        {
            get { return _transaction_Date; }
            set
            {
                if (_transaction_Date != value)
                {
                    _transaction_Date = value;
                    OnPropertyChanged(nameof(TransactionDate));
                }
            }
        }
        public string BranchPlant
        {
            get { return _branch_Plant; }
            set
            {
                if (_branch_Plant != value)
                {
                    _branch_Plant = value;
                    OnPropertyChanged(nameof(BranchPlant));
                }
            }
        }
        public string Quantity
        {
            get { return _quantity; }
            set
            {
                if (_quantity != value)
                {
                    _quantity = value;
                    OnPropertyChanged(nameof(Quantity));
                }
            }
        }
        public string Trans_UoM
        {
            get { return _trans_UoM; }
            set
            {
                if (_trans_UoM != value)
                {
                    _trans_UoM = value;
                    OnPropertyChanged(nameof(Trans_UoM));
                }
            }
        }
        public string Unit_Cost
        {
            get { return _unit_Cost; }
            set
            {
                if (_unit_Cost != value)
                {
                    _unit_Cost = value;
                    OnPropertyChanged(nameof(Unit_Cost));
                }
            }
        }
        public string Extended_Cost
        {
            get { return _extended_Cost; }
            set
            {
                if (_extended_Cost != value)
                {
                    _extended_Cost = value;
                    OnPropertyChanged(nameof(Extended_Cost));
                }
            }
        }
        public string Lot_Serial
        {
            get { return _lot_Serial; }
            set
            {
                if (_lot_Serial != value)
                {
                    _lot_Serial = value;
                    OnPropertyChanged(nameof(Lot_Serial));
                }
            }
        }

        public string Location
        {
            get { return _location; }
            set
            {
                if (_location != value)
                {
                    _location = value;
                    OnPropertyChanged(nameof(Location));
                }
            }
        }
        public string Lot_Status_Code
        {
            get { return _lot_Status_Code; }
            set
            {
                if (_lot_Status_Code != value)
                {
                    _lot_Status_Code = value;
                    OnPropertyChanged(nameof(Lot_Status_Code));
                }
            }
        }
        public string Order_Number
        {
            get { return _order_Number; }
            set
            {
                if (_order_Number != value)
                {
                    _order_Number = value;
                    OnPropertyChanged(nameof(Order_Number));
                }
            }
        }

        public string Order_Ty
        {
            get { return _order_Ty; }
            set
            {
                if (_order_Ty != value)
                {
                    _order_Ty = value;
                    OnPropertyChanged(nameof(Order_Ty));
                }
            }
        }
        public string Order_Co
        {
            get { return _oprder_Co; }
            set
            {
                if (_oprder_Co != value)
                {
                    _oprder_Co = value;
                    OnPropertyChanged(nameof(Order_Co));
                }
            }
        }
        public string LineNum
        {
            get { return _lineNum; }
            set
            {
                if (_lineNum != value)
                {
                    _lineNum = value;
                    OnPropertyChanged(nameof(LineNum));
                }
            }
        }

        public string Class_Code
        {
            get { return _class_Code; }
            set
            {
                if (_class_Code != value)
                {
                    _class_Code = value;
                    OnPropertyChanged(nameof(Class_Code));
                }
            }
        }
        public string GL_Date
        {
            get { return _gL_Date; }
            set
            {
                if (_gL_Date != value)
                {
                    _gL_Date = value;
                    OnPropertyChanged(nameof(GL_Date));
                }
            }

        }
        public string Supplier_Lot_Number
        {
            get { return _supplier_Lot_Number; }
            set
            {
                if (_supplier_Lot_Number != value)
                {
                    _supplier_Lot_Number = value;
                    OnPropertyChanged(nameof(Supplier_Lot_Number));
                }
            }
        }

        public string Trex
        {
            get { return _trex; }
            set
            {
                if (_trex != value)
                {
                    _trex = value;
                    OnPropertyChanged(nameof(Trex));
                }
            }
        }

        public string FT
        {
            get { return _fT; }
            set
            {
                if (_fT != value)
                {
                    _fT = value;
                    OnPropertyChanged(nameof(FT));
                }
            }
        }

        #endregion

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            Type viewModelType = GetType();
            foreach (PropertyInfo propertyInfo in viewModelType.GetProperties())
            {
                string propertyName = propertyInfo.Name;
                string propertyValue = propertyInfo.GetValue(this)?.ToString();

                yield return new KeyValuePair<string, string>(propertyName, propertyValue);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }



    }
}
