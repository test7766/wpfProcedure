using DevExpress.Xpf.Grid;
using Optimapharm.PSC.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using System.Data;
using Optimapharm.PSC.Data;
using System.Data.Entity.Infrastructure;
using System.Data.Entity;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Bars;
using Optimapharm.PSC.PurchasingManager.Windows.FindRestOfItemsWindows.ClassHelper;
using System.Reflection;

namespace Optimapharm.PSC.PurchasingManager.Windows.FindRestOfItemsWindows
{

    public partial class FindRestOfItemsMainWindow : Window
    {
        pr_GetItemLedger_Result[] FoundResltProcedure;
        public INotifyPropertyChangedPropertyChanged viewModel;
        private int pageSize = 5;
        private int currentPage = 1;
        public static List<pr_GetItemLedger_Result> RestitemMoveData = null;
        public static pr_GetItemLedger_Result[] SerachItemMoveDeatail = null;
        public pr_GetItemLedger_Result[] findresult2;
     //  pr_GetItemLedger_Result[] searchResultFound;
        public FindRestOfItemsMainWindow(string ITM, string _mcu, string _LOCN, string _LOTN)
        {
            viewModel = new INotifyPropertyChangedPropertyChanged();
            InitializeComponent();
            ItemsMoveRest.IsEnabled = false;
            txtEditITM.Text = ITM;                           //id товара
            txtEditMCU.Text = _mcu;                          //id склада         char(12)
            txtEditLOTN.Text = _LOTN.Trim();                  //партия            char(30)
            txteditLOCN.Text = _LOCN.Trim();                   // Место склада     char(20)

            //ResourceDictionaryFindRestOfItemsWindows resourceDictionaryFindRestOfItemsWindows = new ResourceDictionaryFindRestOfItemsWindows();
            //Resources.MergedDictionaries.Add(resourceDictionaryFindRestOfItemsWindows);
            //FindRestOfItemLoadingDecorator.KeyDown += ButtonEdit_KeyDown;
            //// FindRestOfItemLoadingDecorator.EditValueChanged += ButtonEdit_EditValueChanged;
            //FindRestOfItemLoadingDecorator.LostFocus += Transaction_Date_LostFocus;
        }

       


        private void UpdateGridData(pr_GetItemLedger_Result[] currntData)
        {
            int startIndex = (currentPage - 1) * pageSize;
            int endIndex = Math.Min(startIndex + pageSize, currntData.Length);
            var pageData = currntData.Skip(startIndex).Take(endIndex - startIndex).ToList();
            ItemsMoveRest.ItemsSource = pageData;
            pageInfoText.Text = $"Сторінка {currentPage} iз {Math.Ceiling((double)currntData.Length / pageSize)}";
        }

        private void UpdateGridDataSearch(pr_GetItemLedger_Result[] o)
        {
            int startIndex = (currentPage - 1) * pageSize;
            int endIndex = Math.Min(startIndex + pageSize, o.Length);

            var pageData = o.Skip(startIndex).Take(endIndex - startIndex).ToList();

            ItemsMoveRest.ItemsSource = pageData;
            pageInfoText.Text = $"Сторінка {currentPage} iз {Math.Ceiling((double)o.Length / pageSize)}";
        }
        //EXEL export
        private void ExportSelectFindRestOfItems_Executed(object sender, ExecutedRoutedEventArgs e)
        {

            try
            {
                var openWinwow = new SaveFileDialog
                {
                    FileName = $"Переміщення  складу_{txtEditMCU.Text.Trim()}" +
                    $"Товару_{txtEditMCU.Text.Trim()}" +
                    $"Місце_{txtEditLOTN.Text.Trim()}" +
                    $"Партія_{txteditLOCN.Text.Trim()} - ",
                    Filter = "Excel файл|*.xlsx"
                };

                if (!(openWinwow.ShowDialog() ?? false))
                    return;


                List<GridColumn> col_prev_state = new List<GridColumn>();
                foreach (var col in ItemsMoveRest?.Columns.Where(p => !p.Visible))
                {
                    col_prev_state.Add(col);
                    col.Visible = true;
                }


                ExcelHelpers.Export(ItemsMoveRest.View, openWinwow.FileName);
                if ((col_prev_state?.Count ?? 0) > 0)
                    foreach (var col in col_prev_state)
                    {
                        col.Visible = false;
                    }

            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message}");
            }

        }
        //search
        private void BTN_Search_History_Of_Remnants_Click(object sender, ItemClickEventArgs e)
        {

            if (string.IsNullOrEmpty(txtEditLOTN.Text) && string.IsNullOrEmpty(txtEditLOTN.Text))
            {
                MessageBox.Show("Або місце або партія повинні бути вказані!");
                return;
            }



            int.TryParse(txtEditITM.Text.ToString(), out int ITM);      //id товара         int
            string MCU = txtEditMCU.Text.ToString();                //id склада         char(12)
            string LOTN = txtEditLOTN.Text.ToString();              //партия            char(30)
            string LOCN = txteditLOCN.Text.ToString();              // Место склада     char(20)


            FindRestOfItemLoadingDecorator.IsSplashScreenShown = true;
            RestitemMoveData = GetItemDetails(ITM, MCU, LOTN, LOCN).ToList();
            FindRestOfItemLoadingDecorator.IsSplashScreenShown = false;

            if (!RestitemMoveData.Any())
                MessageBox.Show($"Історії переміщень не знайденно!", "Увага!", MessageBoxButton.OK, MessageBoxImage.Warning);
            else
            {
                ItemsMoveRest.IsEnabled = true;
                PanelFiltrPagination.IsEnabled = true;
                ItemsMoveRest.ItemsSource = RestitemMoveData;
                (ItemsMoveRest.View as TableView).BestFitColumns();
            }

            UpdateGridData(SerachItemMoveDeatail);
        }

        private void BTN_Clear_TableeResult_Click(object sender, ItemClickEventArgs e) {
            ItemsMoveRest.ItemsSource = null;
            PanelFiltrPagination.IsEnabled = false;
            ItemsMoveRest.IsEnabled = false;
        } 
        //clearGrid
        private void Refresh_CanExecute(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = true;
        //export
        private void ExportSelectFindRestOfItems_CanExecute(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = ItemsMoveRest.ItemsSource != null;


        // GET Data 

        public static pr_GetItemLedger_Result[] GetItemDetails(int? ITM, string mCU, string lOTN, string lOCN)
        {
            try
            {
                using (var entity = new JDE_PRODEntities())
                {
                    ((IObjectContextAdapter)entity).ObjectContext.CommandTimeout = 200;
                    SerachItemMoveDeatail = entity.pr_GetItemLedger(ITM, mCU, lOCN: lOCN, lOTN: lOTN)?.ToArray();

                }
                if (SerachItemMoveDeatail == null) return null;
                return SerachItemMoveDeatail;
            }
            catch (Exception ex) { throw ex; }
        }

        private void BTN_Home_Click(object sender, ItemClickEventArgs e)
        {
            BTN_Next_Name.IsEnabled = true;
            BTN_End_Name.IsEnabled = true;

            BTN_Previous_Name.IsEnabled = false;
            BTN_Home_Name.IsEnabled = false;
            if (FoundResltProcedure != null && FoundResltProcedure.Any())
            {
                currentPage = 1;
                UpdateGridData(FoundResltProcedure);
            }
            else
            {
                currentPage = 1;
                UpdateGridData(SerachItemMoveDeatail);
            }

        }

        //PREVIOUS BTN
        private void BTN_Previous_Click(object sender, ItemClickEventArgs e)
        {
            BTN_Next_Name.IsEnabled = true;
            BTN_End_Name.IsEnabled = true;
            if (currentPage > 1)
            {

                if (FoundResltProcedure != null && FoundResltProcedure.Any())
                {
                    currentPage--;
                    UpdateGridData(FoundResltProcedure);
                }
                else
                {
                    currentPage--;
                    UpdateGridData(SerachItemMoveDeatail);
                }
            }
            else
            {
                BTN_Home_Name.IsEnabled = false;
                BTN_Previous_Name.IsEnabled = false;

            }
        }

        //NEXT BTN
        private void BTN_Next_Click(object sender, ItemClickEventArgs e)
        {

            //if FoundResltProcedure = FoundResltProcedure or SerachItemMoveDeatail
            if (currentPage < Math.Ceiling((double)((FoundResltProcedure != null && FoundResltProcedure.Any()) ? FoundResltProcedure : SerachItemMoveDeatail).Length / pageSize))
            {
                BTN_Previous_Name.IsEnabled = true;
                BTN_Home_Name.IsEnabled = true;
                if (FoundResltProcedure != null && FoundResltProcedure.Any())
                {
                    currentPage++;
                    UpdateGridData(FoundResltProcedure);
                }
                else
                {
                    currentPage++;
                    UpdateGridData(SerachItemMoveDeatail);
                }
            }
            else
            {
                BTN_Previous_Name.IsEnabled = true;
                BTN_Home_Name.IsEnabled = true;
                BTN_Next_Name.IsEnabled = false;
                BTN_End_Name.IsEnabled = false;
            }
        }
        private void BTN_End_Click(object sender, ItemClickEventArgs e)
        {
            BTN_Next_Name.IsEnabled = false;
            BTN_End_Name.IsEnabled = false;
            BTN_Home_Name.IsEnabled = true; //<<
            BTN_Previous_Name.IsEnabled = true;//<
            if (FoundResltProcedure != null && FoundResltProcedure.Any())
            {
                currentPage = (int)Math.Ceiling((double)FoundResltProcedure.Length / pageSize);
                UpdateGridData(FoundResltProcedure);
            }
            else
            {
                currentPage = (int)Math.Ceiling((double)SerachItemMoveDeatail.Length / pageSize);
                UpdateGridData(SerachItemMoveDeatail);
            }

        }

   


        ///user click (if found item)--->HandleSearchResult
        private void ShowUsercount_ItemClick(object sender, ItemClickEventArgs e) =>
            HandleSearchResult((FoundResltProcedure != null && FoundResltProcedure.Any()) ? FoundResltProcedure : SerachItemMoveDeatail);



        //Resetsearch
        private void ResetUserSearch_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            BTN_Home_Name.IsEnabled = false;
            BTN_Previous_Name.IsEnabled = false;
            BTN_Next_Name.IsEnabled = true;
            BTN_End_Name.IsEnabled = true;

            pageSize = 5;
            currentPage = 1;
            SplitEdita.MinValue = 5;

            FoundResltProcedure = null;
            CheckMax.IsChecked = false;

            DependencyPropertyClass DPObjects = (DependencyPropertyClass)Resources["KeyDependency"];
            SetPropertiesToNull(DPObjects);

            UpdateGridData(SerachItemMoveDeatail);
        }


        ////DependencyObjects_SET_NuLL
        public void SetPropertiesToNull(DependencyObject obj)
        {
            Type objectType = obj.GetType();

            PropertyInfo[] properties = objectType.GetProperties();

            foreach (var property in properties)
            {
                if (property.PropertyType.IsGenericType &&
                    property.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    property.SetValue(obj, null);
                }
            }
        }




        //comboBox check/uncheck
        private void CheckMax_EditValueChanged_1(object sender, EditValueChangedEventArgs e) => SplitEdita.IsEnabled = CheckMax.IsChecked == false ? true : false;



        private void HandleSearchResult(pr_GetItemLedger_Result[] _getTemplateDataFullOrEmpty)
        {

            if (CheckMax.IsChecked != true)
            {
                int.TryParse(SplitEdita.Text, out pageSize);
                currentPage = 1;
                UpdateGridDataSearch(_getTemplateDataFullOrEmpty);
                tolbarArrow.IsEnabled = true;
            }
            else
            {
                currentPage = 1;
                pageSize = _getTemplateDataFullOrEmpty.Length;
                UpdateGridDataSearch(_getTemplateDataFullOrEmpty);
                tolbarArrow.IsEnabled = false;
            }
        }



        //public void ChekSwitch(object sender, KeyEventArgs e, TextEdit itemdata, ButtonEdit btnedit, bool isClear = false)
        //{

        //    switch (itemdata.Name)
        //    {
        //        case "Document_Number":
        //            if (btnedit.EditValue != null)
        //            {
        //                viewModel.DocumentNumber = btnedit.EditValue.ToString();
        //            }
        //            else
        //            {
        //                viewModel.DocumentNumber = string.Empty;
        //            }
        //            break;
        //        case "Doc_Type":
        //            if (btnedit.EditValue != null) { viewModel.DocType = btnedit.EditValue.ToString(); }
        //            else { viewModel.DocType = string.Empty; }
        //            break;
        //        case "Doc_Co":
        //            if (btnedit.EditValue != null) { viewModel.DocCo = btnedit.EditValue.ToString(); }
        //            else { viewModel.DocCo = string.Empty; }
        //            break;
        //        case "Transaction_Date":
        //            if (btnedit.EditValue != null) { viewModel.TransactionDate = btnedit.EditValue.ToString(); }
        //            else { viewModel.TransactionDate = string.Empty; }
        //            break;
        //        case "Branch_Plant":
        //            if (btnedit.EditValue != null) { viewModel.BranchPlant = btnedit.EditValue.ToString(); }
        //            else { viewModel.BranchPlant = string.Empty; }
        //            break;
        //        case "Quantity":
        //            if (btnedit.EditValue != null) { viewModel.Quantity = btnedit.EditValue.ToString(); }
        //            else { viewModel.Quantity = string.Empty; }
        //            break;
        //        case "Trans_UoM":
        //            if (btnedit.EditValue != null) { viewModel.Trans_UoM = btnedit.EditValue.ToString(); }
        //            else { viewModel.Trans_UoM = string.Empty; }
        //            break;
        //        case "Unit_Cost":
        //            if (btnedit.EditValue != null) { viewModel.Unit_Cost = btnedit.EditValue.ToString(); }
        //            else { viewModel.Unit_Cost = string.Empty; }
        //            break;
        //        case "Extended_Cost":
        //            if (btnedit.EditValue != null) { viewModel.Extended_Cost = btnedit.EditValue.ToString(); }
        //            else { viewModel.Extended_Cost = string.Empty; }
        //            break;
        //        case "Lot_Serial":
        //            if (btnedit.EditValue != null) { viewModel.Lot_Serial = btnedit.EditValue.ToString(); }
        //            else { viewModel.Lot_Serial = string.Empty; }
        //            break;
        //        case "Location":
        //            if (btnedit.EditValue != null) { viewModel.Location = btnedit.EditValue.ToString(); }
        //            else { viewModel.Location = string.Empty; }
        //            break;
        //        case "Lot_Status_Code":
        //            if (btnedit.EditValue != null) { viewModel.Lot_Status_Code = btnedit.EditValue.ToString(); }
        //            else { viewModel.Lot_Status_Code = string.Empty; }
        //            break;
        //        case "Order_Number":
        //            if (btnedit.EditValue != null) { viewModel.Order_Number = btnedit.EditValue.ToString(); }
        //            else { viewModel.Order_Number = string.Empty; }
        //            break;
        //        case "Order_Ty":
        //            if (btnedit.EditValue != null) { viewModel.Order_Ty = btnedit.EditValue.ToString(); }
        //            else { viewModel.Order_Ty = string.Empty; }
        //            break;
        //        case "Order_Co":
        //            if (btnedit.EditValue != null) { viewModel.Order_Co = btnedit.EditValue.ToString(); }
        //            else { viewModel.Order_Co = string.Empty; }
        //            break;
        //        case "LineNum":
        //            if (btnedit.EditValue != null) { viewModel.LineNum = btnedit.EditValue.ToString(); }
        //            else { viewModel.LineNum = string.Empty; }
        //            break;
        //        case "Class_Code":
        //            if (btnedit.EditValue != null) { viewModel.Class_Code = btnedit.EditValue.ToString(); }
        //            else { viewModel.Class_Code = string.Empty; }
        //            break;
        //        case "GL_Date":
        //            if (btnedit.EditValue != null) { viewModel.GL_Date = btnedit.EditValue.ToString(); }
        //            else { viewModel.GL_Date = string.Empty; }
        //            break;
        //        case "Supplier_Lot_Number":
        //            if (btnedit.EditValue != null) { viewModel.Supplier_Lot_Number = btnedit.EditValue.ToString(); }
        //            else { viewModel.Supplier_Lot_Number = string.Empty; }
        //            break;
        //        case "Trex":
        //            if (btnedit.EditValue != null) { viewModel.Trex = btnedit.EditValue.ToString(); }
        //            else { viewModel.Trex = string.Empty; }
        //            break;
        //        case "FT":
        //            if (btnedit.EditValue != null) { viewModel.FT = btnedit.EditValue.ToString(); }
        //            else { viewModel.FT = string.Empty; }
        //            break;
        //        default:
        //            break;

        //    }
        //}



        //checking and clearing extra characters
        //private void PreviewKeyDownClick(object sender, KeyEventArgs e)
        //{

        //    if (e.Key == Key.Enter)
        //    {
        //        ChekFieldAndFindItem();
        //    }
        //    else
        //    {
        //        TextEdit findtypeofvariable = sender as TextEdit;
        //        ChekSwitch(sender, e, findtypeofvariable, null, false);
        //    }

        //}


        //click for datetime 
        //private void Transaction_Date_LostFocus(object sender, RoutedEventArgs e)
        //{
        //    DateEdit filterTextEditDocCo = sender as DateEdit;
        //    viewModel.TransactionDate = (sender as DateEdit)?.Text;

        //}
        //private void GL_Date_LostFocus(object sender, RoutedEventArgs e)
        //{
        //    DateEdit filterTextEditDocCo = sender as DateEdit;
        //    viewModel.GL_Date = (sender as DateEdit)?.Text;
        //}


        // strings -----------> casting to type (if IsNullOrEmpty) and execute query
        //public void ChekFieldAndFindItem()
        //{
        //    pr_GetItemLedger_Result _pr_GetItemLedger_Cast = new pr_GetItemLedger_Result();
        //    foreach (KeyValuePair<string, string> property in viewModel)
        //    {
        //        string propertyName = property.Key;
        //        string propertyValue = property.Value;

        //        if (!string.IsNullOrEmpty(property.Value))

        //        {
        //            switch (property.Key)
        //            {
        //                case "_document_Number":
        //                    double docNum;
        //                    double.TryParse(propertyValue, out docNum);
        //                    _pr_GetItemLedger_Cast.Document_Number = docNum;
        //                    break;
        //                case "_doc_Type":
        //                    _pr_GetItemLedger_Cast.Doc_Type = propertyValue;
        //                    break;
        //                case "_doc_Co":
        //                    _pr_GetItemLedger_Cast.Doc_Co = propertyValue;
        //                    break;
        //                case "_transaction_Date":
        //                    DateTime transactionDate;
        //                    DateTime.TryParseExact(propertyValue, "MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out transactionDate);
        //                    _pr_GetItemLedger_Cast.Transaction_Date = transactionDate;
        //                    break;
        //                case "_branch_Plant":
        //                    _pr_GetItemLedger_Cast.Branch_Plant = propertyValue;
        //                    break;
        //                case "_quantity":
        //                    _pr_GetItemLedger_Cast.Quantity = Convert.ToDouble(propertyValue);
        //                    break;
        //                case "_trans_UoM":
        //                    _pr_GetItemLedger_Cast.Trans_UoM = propertyValue;
        //                    break;
        //                case "_unit_Cost":
        //                    _pr_GetItemLedger_Cast.Unit_Cost = Convert.ToDouble(propertyValue);
        //                    break;
        //                case "_extended_Cost":
        //                    _pr_GetItemLedger_Cast.Extended_Cost = Convert.ToDouble(propertyValue);
        //                    break;
        //                case "_lot_Serial":
        //                    _pr_GetItemLedger_Cast.Lot_Serial = propertyValue;
        //                    break;
        //                case "_location":
        //                    _pr_GetItemLedger_Cast.Location = propertyValue;
        //                    break;
        //                case "_lot_Status_Code":
        //                    _pr_GetItemLedger_Cast.Lot_Status_Code = propertyValue;
        //                    break;
        //                case "_order_Number":
        //                    _pr_GetItemLedger_Cast.Order_Number = Convert.ToDouble(propertyValue);
        //                    break;
        //                case "_order_Ty":
        //                    _pr_GetItemLedger_Cast.Order_Ty = propertyValue;
        //                    break;
        //                case "_order_Co":
        //                    _pr_GetItemLedger_Cast.Order_Co = propertyValue;
        //                    break;
        //                case "_lineNum":
        //                    _pr_GetItemLedger_Cast.LineNum = Convert.ToDouble(propertyValue);
        //                    break;
        //                case "_class_Code":
        //                    _pr_GetItemLedger_Cast.Class_Code = propertyValue;
        //                    break;
        //                case "_gL_Date":
        //                    DateTime _GL_Date;
        //                    DateTime.TryParseExact(propertyValue, "MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out _GL_Date);
        //                    _pr_GetItemLedger_Cast.GL_Date = _GL_Date;
        //                    break;
        //                case "_supplier_Lot_Number":
        //                    _pr_GetItemLedger_Cast.Supplier_Lot_Number = propertyValue;
        //                    break;
        //                case "_trex":
        //                    _pr_GetItemLedger_Cast.Trex = propertyValue;
        //                    break;
        //                case "_fT":
        //                    _pr_GetItemLedger_Cast.FT = propertyValue;
        //                    break;
        //                default:
        //                    break;
        //            }

        //        }

        //    }

        //    searchResultFound = SerachItemMoveDeatail.AsQueryable().Where(QueryableExtensions.FilterKey(_pr_GetItemLedger_Cast)).ToArray();

        //    if (searchResultFound.Any())
        //    {
        //        HandleSearchResult(searchResultFound);
        //    }
        //    else
        //    {
        //        MessageBox.Show("За вашим запитом результатів не знайдено");
        //        ItemsMoveRest.ItemsSource = null;
        //    }

        //}


        //private void ButtonEdit_EditValueChanged(object sender, EditValueChangedEventArgs e)
        //{




        //    //  var newvalue = _Document_Number.ToString();



        //    var buttonEdit = (ButtonEdit)sender;
        //    var curValue = buttonEdit.Text;
        //    if (string.IsNullOrEmpty(buttonEdit.Text)) { ChekSwitch(null, null, buttonEdit as TextEdit, buttonEdit, true); }
        //    else { ChekSwitch(null, null, buttonEdit as TextEdit, buttonEdit, false); }
        //}

        pr_GetItemLedger_Result _pr_GetItemLedgerFoundField;
        public void ButtonEdit_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) { CheckFieldAndSeedData(); }
        }


        void CheckFieldAndSeedData()
        {
            _pr_GetItemLedgerFoundField = new pr_GetItemLedger_Result();
            DependencyPropertyClass dpSample = (DependencyPropertyClass)Resources["KeyDependency"];

            foreach (var item in dpSample)
            {

                if (item.Value != null && item.Value.ToString() != string.Empty)

                {
                    switch (item.Key)
                    {
                        case "Document_Number":
                            _pr_GetItemLedgerFoundField.Document_Number = (double)item.Value;
                            break;
                        case "Doc_Type":
                            _pr_GetItemLedgerFoundField.Doc_Type = (string)item.Value;
                            break;
                        case "Doc_Co":
                            _pr_GetItemLedgerFoundField.Doc_Co = (string)item.Value;
                            break;
                        case "Transaction_Date":
                            _pr_GetItemLedgerFoundField.Transaction_Date = (DateTime)item.Value;
                            break;
                        case "Branch_Plant":
                            _pr_GetItemLedgerFoundField.Branch_Plant = (string)item.Value;
                            break;
                        case "Quantity":
                            _pr_GetItemLedgerFoundField.Quantity = (double)item.Value;
                            break;
                        case "Trans_UoM":
                            _pr_GetItemLedgerFoundField.Trans_UoM = (string)item.Value;
                            break;
                        case "Unit_Cost":
                            _pr_GetItemLedgerFoundField.Unit_Cost = (double)item.Value;
                            break;
                        case "Extended_Cost":
                            _pr_GetItemLedgerFoundField.Extended_Cost = (double)item.Value;
                            break;
                        case "Lot_Serial":
                            _pr_GetItemLedgerFoundField.Lot_Serial = (string)item.Value;
                            break;
                        case "Location":
                            _pr_GetItemLedgerFoundField.Location = (string)item.Value;
                            break;
                        case "Lot_Status_Code":
                            _pr_GetItemLedgerFoundField.Lot_Status_Code = (string)item.Value;
                            break;
                        case "Order_Number":
                            _pr_GetItemLedgerFoundField.Order_Number = (double)item.Value;
                            break;
                        case "Order_Ty":
                            _pr_GetItemLedgerFoundField.Order_Ty = (string)item.Value;
                            break;
                        case "Order_Co":
                            _pr_GetItemLedgerFoundField.Order_Co = (string)item.Value;
                            break;
                        case "LineNum":
                            _pr_GetItemLedgerFoundField.LineNum = (double)item.Value;
                            break;
                        case "Class_Code":
                            _pr_GetItemLedgerFoundField.Class_Code = (string)item.Value;
                            break;
                        case "GL_Date":
                            _pr_GetItemLedgerFoundField.GL_Date = (DateTime)item.Value;
                            break;
                        case "Supplier_Lot_Number":
                            _pr_GetItemLedgerFoundField.Supplier_Lot_Number = (string)item.Value;
                            break;
                        case "Trex":
                            _pr_GetItemLedgerFoundField.Trex = (string)item.Value;
                            break;
                        case "FT":
                            _pr_GetItemLedgerFoundField.FT = (string)item.Value;
                            break;
                        default:
                            break;
                    }
                }
            }
             FoundResltProcedure = SerachItemMoveDeatail.AsQueryable().Where(QueryableExtensions.FilterKey(_pr_GetItemLedgerFoundField)).ToArray();
            if (FoundResltProcedure.Any())
            {
                HandleSearchResult(FoundResltProcedure);
            }
            else
            {
                MessageBox.Show("За вашим запитом результатів не знайдено");
                ItemsMoveRest.ItemsSource = null;
            }
        }



        //Direct way create DependencyProperty
        //для TextEdit
        public DateTime? NecessaryField
        {
            get { return (DateTime?)GetValue(RecepitTimeDP); }
            set { SetValue(RecepitTimeDP, value); }
        }

        public static readonly DependencyProperty RecepitTimeDP = DependencyProperty.Register("NecessaryField", typeof(DateTime?), typeof(FindRestOfItemsMainWindow));


    }
}
