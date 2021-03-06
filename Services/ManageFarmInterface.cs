﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Itsomax.Data.Infrastructure.Data;
using Itsomax.Module.Core.Data;
using Itsomax.Module.Core.Extensions;
using Itsomax.Module.Core.Interfaces;
using Itsomax.Module.Core.ViewModels;
using Itsomax.Module.FarmSystemCore.Interfaces;
using Itsomax.Module.FarmSystemCore.Models;
using Itsomax.Module.FarmSystemCore.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace Itsomax.Module.FarmSystemCore.Services
{
    public class ManageFarmInterface : IManageFarmInterface
    {
        private readonly IRepository<Locations> _location;
        private readonly IRepository<CostCenter> _costCenter;
        private readonly IRepository<Products> _products;
        private readonly ItsomaxDbContext _context;
        private readonly IRepository<CostCenterProducts> _costCenterProduct;
        private readonly IRepository<BaseUnits> _baseUnits;
        private readonly IRepository<Consumptions> _consumption;
        private readonly IRepository<ConsumptionDetails> _consumptionDetails;
        private readonly ILogginToDatabase _logger;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IRepository<ProductTypes> _productTypes;
        private readonly IRepository<Folio> _folio;
        
        public ManageFarmInterface(IRepository<Locations> location,ILogginToDatabase logger,
            IRepository<CostCenter> costCenter,
            IRepository<Products> products,ItsomaxDbContext context,IRepository<Consumptions> consumption,
            IRepository<CostCenterProducts> costCenterProduct,IRepository<BaseUnits> baseUnits,
            IRepository<ConsumptionDetails> consumptionDetails, IHostingEnvironment hostingEnvironment,
            IRepository<ProductTypes> productTypes, IRepository<Folio> folio)
        {
            _location = location;
            _logger = logger;
            _costCenter = costCenter;
            _products = products;
            _context = context;
            _costCenterProduct = costCenterProduct;
            _baseUnits = baseUnits;
            _consumption = consumption;
            _consumptionDetails = consumptionDetails;
            _hostingEnvironment = hostingEnvironment;
            _productTypes = productTypes;
            _folio = folio;
        }
        
        

        public async Task<SystemSucceededTask> AddBaseUnit(BaseUnitViewModel model, string username)
        {
            var baseUnit = new BaseUnits
            {
                CreatedOn = DateTimeOffset.Now,
                Active = model.Active,
                Description = model.Description,
                Name = model.Name,
                Value = model.Value
            };
            _baseUnits.Add(baseUnit);
            try
            {
                await _baseUnits.SaveChangesAsync();
                _logger.InformationLog("Base unit " + model.Name + " created successfully", "Create Base Unit",
                    string.Empty, username);
                return SystemSucceededTask.Success("Base unit " + model.Name + " created successfully");
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex.Message, "Create Base Unit", ex.InnerException.Message, username);
                return SystemSucceededTask.Failed("Base unit " + model.Name + " created successfully",
                    ex.InnerException.Message, true, false);
            }
            
        }

        public async Task<SystemSucceededTask> EditBaseUnit(BaseUnitEditViewModel model, string userName)
        {
            var oldBaseUnit = _baseUnits.GetById(model.Id);
            if (oldBaseUnit == null)
            {
                _logger.ErrorLog("Base Unit not found","Base Unit Edit",string.Empty,userName);
                return SystemSucceededTask.Failed("Base Unit not found", string.Empty, false, true);
            }
            
            oldBaseUnit.Active = model.Active;
            oldBaseUnit.Name = model.Name;
            oldBaseUnit.Description = model.Description;
            oldBaseUnit.Value = model.Value;
            oldBaseUnit.UpdatedOn = DateTimeOffset.Now;

            try
            {
                await _baseUnits.SaveChangesAsync();
                _logger.InformationLog("Base Unit " + model.Name + " edited successfully", "Base Unit Edit",
                    string.Empty, userName);
                return SystemSucceededTask.Success("Base Unit " + model.Name + " edited successfully");
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex.Message, "Base Unit Edit",ex.InnerException.Message, userName);
                return SystemSucceededTask.Failed("Base Unit " + model.Name + " edited unsuccessful",
                    ex.InnerException.Message, true, false);
            }
        }


        public IEnumerable<BaseUnits> GetBaseUnitList()
        {
            return _baseUnits.Query().ToList();
        }

        public BaseUnits GetBaseUnitById(long id)
        {
            return _baseUnits.Query().FirstOrDefault(x => x.Id == id);
        }
        public BaseUnits GetBaseUnitByValue(string value)
        {
            return _baseUnits.Query().FirstOrDefault(x => x.Value == value);
        }

        public async Task<SystemSucceededTask> AddLocation(LocationViewModel model,string username)
        {
            if (!ValidateLocationCode(model.Code))
            {
                _logger.ErrorLog("Code for location already exists","Create Location",string.Empty,username);
                return SystemSucceededTask.Failed("Location " + model.Name + " code already exists", string.Empty,
                    false, true);
            }

            var location = new Locations()
            {
                Active = model.Active,
                Code = model.Code,
                CreatedOn = DateTimeOffset.Now,
                Description = model.Description,
                Name = model.Name,
            };
            _location.Add(location);
            
            try
            {
                
                await _location.SaveChangesAsync();
                _logger.InformationLog("Location " + model.Name + " created successfully", "Create Location",
                    string.Empty, username);
                return SystemSucceededTask.Success("Location " + model.Name + " created successfully");
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex.Message,"Create Location",ex.InnerException.Message,username);
                return SystemSucceededTask.Failed("Location " + model.Name + " created unsuccessfully",
                    ex.InnerException.Message, true, false);
            }
        }

        public async Task<SystemSucceededTask> EditLocation(LocationEditViewModel model, string username)
        {
            if (model == null)
            {
                _logger.ErrorLog("Location not found","Edit Location ",string.Empty,username);
                return SystemSucceededTask.Failed("Location not found",string.Empty, false, true);
            }

            var location = await _location.Query().FirstOrDefaultAsync(x => x.Id == model.Id);
            location.Active = model.Active;
            location.CreatedOn = location.CreatedOn;
            location.Name = model.Name;
            location.Description = model.Description;

            try
            {
                await _location.SaveChangesAsync();
                _logger.InformationLog("Location " + model.Name + " updated successfully", "Update Location",
                    string.Empty, username);
                return SystemSucceededTask.Success("Location " + model.Name + " updated successfully");
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex.Message,"Update Location",ex.InnerException.Message,username);
                return SystemSucceededTask.Failed("Location " + model.Name + " updated unsuccessful",
                    ex.InnerException.Message, true, false);
            }
        }
        

        public bool EnableDisableLocation(long id,string username)
        {

            var locations = _location.Query().FirstOrDefaultAsync(x => x.Id == id).Result;
            if (locations == null)
                return false;

            switch (locations.Active)
            {
                case true:
                    locations.Active = false;
                    break;
                case false:
                    locations.Active = true;
                    break;
            }

            try
            {
                _location.SaveChanges();
                _logger.InformationLog("Location " + locations.Name + " Enable/Disable successfully",
                    "Enable/Disable Location", string.Empty, username);
                return true;
            }
            catch (SqlException ex)
            {
                _logger.ErrorLog(ex.Message,"Enable/Disable Location",ex.InnerException.Message,username);
                return false;
            }
        }

        private bool ValidateLocationCode(string code)
        {
            var exists = _location.Query().FirstOrDefault(x => x.Code == code);
            return exists == null;
        }

        public IEnumerable<Locations> GetLocation()
        {
            return _location.Query();
        }
        public IEnumerable<Locations> GetActiveLocation()
        {
            return _location.Query().Where(x => x.Active);
        }

        public Locations GetLocationByName(string location)
        {
            return _location.Query().FirstOrDefault(x => x.Name == location);
        }

        public Locations GetLocationById(long id)
        {
            return _location.Query().FirstOrDefault(x => x.Id == id);
        }

        public List<LocationList> GetLocationList(long? costCenterId)
        {
            if (costCenterId == null)
            {
                var locationList = new List<LocationList>();   
                var locList = (from a in GetActiveLocation()
                    select new { LocationId = a.Id, LocationName = a.Name,Selected = false }).ToList();
                locationList.Add(new LocationList
                {
                    LocationId = 0,
                    LocationName = "Select a Location",
                    Selected = true
                });
                foreach (var item in locList)
                {
                    locationList.Add(new LocationList
                    {
                        LocationId = item.LocationId,
                        LocationName = item.LocationName,
                        Selected = false
                    });
                }

                return locationList;
            }
            else
            {
                var costCenter = GetCostCenterById(costCenterId.Value);

                var locationList = new List<LocationList>();
                var locList = (from a in GetActiveLocation()
                    select new { LocationId = a.Id, LocationName = a.Name,Selected = false }).ToList();
                foreach (var item in locList)
                {
                    bool selected = item.LocationId == costCenter.LocationId;
                    locationList.Add(new LocationList
                    {
                        LocationId = item.LocationId,
                        LocationName = item.LocationName,
                        Selected = selected
                    });

                }
                return locationList;

            }
        }        
        
        public async Task<SystemSucceededTask> AddCostCenter(CostCenterViewModel model,string username)
        {
            if (!ValidateCostCenterCode(model.Code))
            {
                _logger.ErrorLog("Cost center " + model.Name + " code already exists", "Create Cost Center",
                    "Code for cost center already exists", username);
                return SystemSucceededTask.Failed("Cost center " + model.Name + " code already exists", string.Empty,
                    false, true);
            }

            var centerCost = new CostCenter()
            {
                Active = model.Active,
                Code = model.Code,
                CreatedOn = DateTimeOffset.Now,
                Description = model.Description,
                WarehouseCode = model.WarehouseCode,
                Name = model.Name,
                LocationId = model.LocationId,
                IsFarming = model.IsFarming,
                IsMeal = model.IsMeal,
                IsMedical = model.IsMedical
            };
            _costCenter.Add(centerCost);

            try
            {
                
                await _costCenter.SaveChangesAsync();
                _logger.InformationLog("Cost Center " + model.Name + " created successfully", "Create Cost Center",
                    string.Empty, username);
                return SystemSucceededTask.Success("Cost Center " + model.Name + " created successfully");
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex.Message,"Create Cost Center",ex.InnerException.Message,username);
                return SystemSucceededTask.Failed("Cost Center " + model.Name + " created unsuccessful",
                    ex.InnerException.Message, true, false);
            }
        }

        public async Task<SystemSucceededTask> EditCostCenter(CostCenterEditViewModel model,string username)
        {
            if (model == null)
            {
                _logger.ErrorLog("Cost Center not found","Edit Cost Center","Cost Center not found",username);
                return SystemSucceededTask.Failed("Cost Center not found",string.Empty, false, true);
            }

            var costCenter = await _costCenter.Query().FirstAsync(x => x.Id == model.Id);
            costCenter.Active = model.Active;
            costCenter.LocationId = model.LocationId;
            costCenter.CreatedOn = costCenter.CreatedOn;
            costCenter.Name = model.Name;
            costCenter.Description = model.Description;
            costCenter.IsFarming = model.IsFarming;
            costCenter.IsMeal = model.IsMeal;
            costCenter.IsMedical = model.IsMedical;
            costCenter.WarehouseCode = model.WarehouseCode;
            try
            {
                await _costCenter.SaveChangesAsync();
                _logger.InformationLog("Cost Center " + model.Name + " updated successfully", "Edit Cost Center",
                    string.Empty, username);
                return SystemSucceededTask.Success("Cost Center " + model.Name + " updated successfully");
            }
            catch (SqlException ex)
            {
                _logger.ErrorLog(ex.Message,"Edit Cost Center",ex.InnerException.Message,username);
                return SystemSucceededTask.Failed("Edit Center " + model.Name + " updated unsuccessfully",
                    ex.InnerException.Message, false, true);
            }
        }

        public bool EnableDisableCostCenter(long id,string username)
        {

            var costCenter = _costCenter.Query().FirstOrDefaultAsync(x => x.Id == id).Result;
            if (costCenter == null)
                return false;

            switch (costCenter.Active)
            {
                case true:
                    costCenter.Active = false;
                    break;
                case false:
                    costCenter.Active = true;
                    break;
            }

            try
            {
                _location.SaveChanges();
                _logger.InformationLog("Cost Center " + costCenter.Name + " Enable/Disable unsuccessful",
                    "Enable/Disable Cost Center", string.Empty, username);
                return true;
            }
            catch (SqlException ex)
            {
                _logger.ErrorLog("Cost Center " + costCenter.Name + " Enable/Disable unsuccessful",
                    "Enable/Disable Cost Center", ex.InnerException.Message, username);
                return false;
            }
        }

        private bool ValidateCostCenterCode(string code)
        {
            var exists = _costCenter.Query().FirstOrDefault(x => x.Code == code);
            return exists == null;
        }
        
        public async Task<SystemSucceededTask> AddProductsToCostCenter(ProductCostCenterViewModel model,
            string username,params string[] selectedProducts)
        {
            //TODO: improve try catch
            var productsCostCenter = _context.Set<CostCenterProductsDetails>()
                .Where(x => x.CostCenterId == model.CostCenterId).ToList();
            if (productsCostCenter.Any())
            {
                _context.Set<CostCenterProductsDetails>().RemoveRange(productsCostCenter);
                await _context.SaveChangesAsync();
            }

            var costCenterProduct = new CostCenterProducts
            {
                Name = model.Name,
                CreatedOn = DateTimeOffset.Now,
                Active = model.Active
            };
            _costCenterProduct.Add(costCenterProduct);
            await _costCenterProduct.SaveChangesAsync();

            IList<CostCenterProductsDetails> costcenter = selectedProducts.Select(GetProductByName)
                .Select(product => new CostCenterProductsDetails
                {
                    CostCenterId = model.CostCenterId,
                    ProductId = product.Id,
                    CostCenterProductsId = costCenterProduct.Id
                }).ToList();

            if (costcenter.Any())
            {
                _context.Set<CostCenterProductsDetails>().AddRange(costcenter);
            }
            
            try
            {
                await _context.SaveChangesAsync();
                _logger.InformationLog("Cost Center " + model.Name + " : product added successfully",
                    "Edit Cost Center", string.Empty, username);
                return SystemSucceededTask.Success("Cost Center " + model.Name + " : product added successfully");
            }
            catch (SqlException ex)
            {
                _logger.ErrorLog(ex.Message,"Edit Cost Center",ex.InnerException.Message,username);
                return SystemSucceededTask.Failed("Cost Center " + model.Name + " product added unsuccessful",
                    ex.InnerException.Message, true, false);
            }
        }

        public string GetCostCenterProductName(long costCenterId)
        {
            var name = (from cc in _costCenter.Query()
                join pd in _context.Set<CostCenterProductsDetails>() on cc.Id equals pd.CostCenterId
                join ccname in _costCenterProduct.Query() on pd.CostCenterProductsId equals ccname.Id
                where cc.Id == costCenterId
                select(ccname.Name)).FirstOrDefault();
            return name;
        }
        
        public bool GetCostCenterProductActive(long costCenterId)
        {
            var active = (from cc in _costCenter.Query()
                join pd in _context.Set<CostCenterProductsDetails>() on cc.Id equals pd.CostCenterId
                join ccname in _costCenterProduct.Query() on pd.CostCenterProductsId equals ccname.Id
                where cc.Id == costCenterId
                select(ccname.Active)).FirstOrDefault();
            return active;
        }

        public IEnumerable<SelectListItem> GetSelectListProducts(long centerCostId)
        {
            return _products.Query().Select(x => new SelectListItem
            {
                Selected = GetProductsForCostCenter(centerCostId).Contains(x.Name),
                Text = x.Name,
                Value = x.Name
            });
        }

        public IList<string> GetProductsForCostCenter(long centerCostId)
        {
            var products = from cc in _costCenter.Query().ToList()
                join pd in _context.Set<CostCenterProductsDetails>().ToList() on cc.Id equals pd.CostCenterId
                join pr in _products.Query().ToList() on pd.ProductId equals pr.Id
                where cc.Id == centerCostId
                select(pr.Name);
            return products.ToList();
        }

        public async Task<SystemSucceededTask> SaveConsumption(long costCenterId, string[] products, string[] values,
            string username,DateTimeOffset? lateCreatedOn)
        {
            var count = products.Length;
            var costCenter = GetCostCenterById(costCenterId);
            var consumptionHeader = new Consumptions
            {
				Name = "Added meal in " + costCenter.Name + " " + DateTimeOffset.Now,
                LateCreatedOn = lateCreatedOn ?? DateTimeOffset.Now
                
                
            };
            _consumption.Add(consumptionHeader);

            if (values.Any(string.IsNullOrEmpty))
            {
                return SystemSucceededTask.Failed("You need to put all fields with a value, if a product has no value put 0","there is empty value entered",false,true);
            }

            try
            {
                await _consumption.SaveChangesAsync();
                for (var i = 0; i < count;  i++)
                {
                    var getProduct = await (from a in _products.Query()
                        join b in _baseUnits.Query() on a.BaseUnitId equals b.Id
                        where a.Name == products[i]
                        select new {a.Id, a.Name, BaseUnit = b.Value}).FirstOrDefaultAsync();
                    var consumptionDetail = new ConsumptionDetails
                    {
                        ConsumptionId = consumptionHeader.Id,
                        CostCenterId = costCenter.Id,
                        WarehouseCode = costCenter.WarehouseCode,
                        ProductId = getProduct.Id,
                        BaseUnit = getProduct.BaseUnit,
                        Weight = Convert.ToDecimal(values[i].Replace(",","."))
                        
                    };
                    _consumptionDetails.Add(consumptionDetail);
                }
                try
                {
                    await _consumptionDetails.SaveChangesAsync();
                    _logger.InformationLog("Consumption " + consumptionHeader.Name + " : created successfully",
                        "Create Consumption", string.Empty, username);
                    return SystemSucceededTask.Success("Consumption " + consumptionHeader.Name +
                                                       " : created successfully");
                }
                catch (SqlException ex)
                {
                    _consumption.Remove(consumptionHeader);
                    await _consumption.SaveChangesAsync();
                    _logger.ErrorLog(ex.Message,"Create Consumption Details",ex.InnerException.Message,username);
                    return SystemSucceededTask.Failed("Consumption " + consumptionHeader.Name + " created unsuccessful",
                        ex.InnerException.Message, true, false);
                }
                
            }
            catch (SqlException ex)
            {
                _logger.ErrorLog(ex.Message,"Create Consumption",ex.InnerException.Message,username);
                return SystemSucceededTask.Failed("Consumption " + consumptionHeader.Name + " created unsuccessfully",
                    ex.InnerException.Message, true, false);
            }
        }

        public async Task<SystemSucceededTask> SaveConsumptionEdit(long consumptionId, string[] products, 
            string[] values,string username)
        {
            var count = products.Length;
            var consumptionHeader = _consumption.Query().FirstOrDefault(x => x.Id == consumptionId);
            var oldConsumptionDetail = _consumptionDetails.Query().Where(x => x.ConsumptionId == consumptionId).ToList();
            var oldConsumptionDate = consumptionHeader.CreatedOn;
            consumptionHeader.CreatedOn = DateTimeOffset.Now;

            if (values.Any(string.IsNullOrEmpty))
            {
                return SystemSucceededTask.Failed("You need to put all fields with a value, if a product has no value put 0", "there is empty value entered", false, true);
            }

            foreach (var item in oldConsumptionDetail)
            {
                _consumptionDetails.Remove(_consumptionDetails.Query().FirstOrDefault(x => x.Id == item.Id));
            }

            
            try
            {
                await _consumption.SaveChangesAsync();
                await _consumptionDetails.SaveChangesAsync();
                try
                {
                    for (int i = 0; i < count; i++)
                    {
                        var getProduct = await (from a in _products.Query()
                            join b in _baseUnits.Query() on a.BaseUnitId equals b.Id
                            where a.Name == products[i]
                            select new {a.Id, a.Name, BaseUnit = b.Value}).FirstOrDefaultAsync();
                        var consumptionDetail = new ConsumptionDetails
                        {
                            ConsumptionId = consumptionHeader.Id,
                            CostCenterId = oldConsumptionDetail.FirstOrDefault().CostCenterId,
                            WarehouseCode = oldConsumptionDetail.FirstOrDefault().WarehouseCode,
                            ProductId = getProduct.Id,
                            BaseUnit = getProduct.BaseUnit,
                            Weight = Convert.ToDecimal(values[i].Replace(",","."))
                        
                        };
                        _consumptionDetails.Add(consumptionDetail);
                    }

                    await _consumptionDetails.SaveChangesAsync();
                    _logger.InformationLog("Consumption " + consumptionHeader.Name + " : updated successfully",
                        "Create Consumption Edit", string.Empty, username);
                    return SystemSucceededTask.Success("Consumption " + consumptionHeader.Name +
                                                       " : updated successfully");
                }
                catch (SqlException ex)
                {
                    var consumptionReverse = _consumption.Query().FirstOrDefault(x => x.Id == consumptionId);
                    consumptionReverse.CreatedOn = oldConsumptionDate;
                    await _consumption.SaveChangesAsync();
                    _logger.ErrorLog(ex.Message,"Create Consumption Edit",ex.InnerException.Message,username);
                    return SystemSucceededTask.Failed(
                        "Consumption " + consumptionHeader.Name + " updated unsuccessfully", ex.InnerException.Message,
                        true, false);
                }
                
            }
            catch (SqlException ex)
            {
                _logger.ErrorLog(ex.Message,"Create Consumption Edit",ex.InnerException.Message,username);
                return SystemSucceededTask.Failed(
                    "Consumption " + consumptionHeader.Name + " updated unsuccessfully", ex.InnerException.Message,
                    true, false);
            }
        }
        

        public IEnumerable<CostCenter> GetCostCenters()
        {
            return _costCenter.Query();
        }

        public IEnumerable<CostCenter> GetActiveCostCenters()
        {
            return _costCenter.Query().Where(x => x.Active);
        }

        public CostCenter GetCostCenterByName(string name)
        {
            return _costCenter.Query().FirstOrDefault(x => x.Name == name);
        }

        public CostCenter GetCostCenterById(long id)
        {
            return _costCenter.Query().FirstOrDefault(x => x.Id == id);
        }

        public IEnumerable<BaseUnits> GetActiveBaseUnits()
        {
            return _baseUnits.Query().Where(x => x.Active == true);
        }

        public List<BaseUnitList> GetBaseUnitList(long? productId)
        {
            if (productId == null)
            {
                var baseUnitLists = new List<BaseUnitList>();   
                var locList = (from a in GetActiveBaseUnits()
                    select new { BaseUnutId = a.Id, BaseUnitName = a.Name,Selected = false }).ToList();
                baseUnitLists.Add(new BaseUnitList
                {
                    BaseUnitId = 0,
                    BaseUnitName = "Select a Base Unit",
                    Selected = true
                });
                foreach (var item in locList)
                {
                    baseUnitLists.Add(new BaseUnitList
                    {
                        BaseUnitId = item.BaseUnutId,
                        BaseUnitName = item.BaseUnitName,
                        Selected = false
                    });
                }

                return baseUnitLists;
            }
            else
            {
                var product = GetProductById(productId.Value);

                var baseUnitLists = new List<BaseUnitList>();
                var locList = (from a in GetActiveBaseUnits()
                    select new { BaseUnitId = a.Id, BaseUnitName = a.Name,Selected = false }).ToList();
                foreach (var item in locList)
                {
                    var selected = item.BaseUnitId == product.BaseUnitId;
                    baseUnitLists.Add(new BaseUnitList
                    {
                        BaseUnitId = item.BaseUnitId,
                        BaseUnitName = item.BaseUnitName,
                        Selected = selected
                    });

                }
                return baseUnitLists;

            }
        }

        public IList<GenericSelectList> GetProductTypeList(long? productId)
        {
            if (productId == null)
            {
                var productTypeList = new List<GenericSelectList>();
                var prodtList = _productTypes.GetAll();
                
                productTypeList.Add(new GenericSelectList
                    {
                        Id = 0,
                        Name = "Select a Product Type",
                        Selected = true
                    });
                productTypeList.AddRange(prodtList.Select(item => new GenericSelectList
                {
                    Id = item.Id, Name = item.Type, Selected = false
                    
                }));
                return productTypeList;
            }
            else
            {
                var product = GetProductById(productId.Value);
                var productTypeList = new List<GenericSelectList>();
                var prodtList = _productTypes.GetAll();
                foreach (var item in prodtList)
                {
                    var Selected = item.Id == product.ProductTypeId;
                    productTypeList.Add(new GenericSelectList
                    {
                        Id = item.Id,
                        Name = item.Type,
                        Selected = Selected
                    });
                    
                }

                return productTypeList;
            }
        }

        public async Task<SystemSucceededTask> AddProduct(ProductViewModel model,string username)
        {
            if (!ValidateProductCode(model.Code))
            {
                _logger.ErrorLog("Product " + model.Name + " code already exists", "Create Product",
                    "Code for product already exists", username);
                return SystemSucceededTask.Failed("Product " + model.Name + " code already exists", string.Empty, false,
                    true);
            }
            var product = new Products
            {
                Active = model.Active,
                BaseUnitId = model.BaseUnitId,
                Code = model.Code,
                Description = model.Description,
                Name = model.Name,
                ProductTypeId = model.ProductTypeId
            };
            _products.Add(product);
            try
            {
                await _products.SaveChangesAsync();
                _logger.InformationLog("Product " + model.Name + " created successfully", "Create Product",
                    string.Empty, username);
                return SystemSucceededTask.Success("Product " + model.Name + " created successfully");

            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex.Message,"Create Product",ex.InnerException.Message,username);
                return SystemSucceededTask.Failed("Product " + model.Name + " created unsuccessfully",
                    ex.InnerException.Message, true, false);
            }
        }

        public async Task<SystemSucceededTask> EditProduct(ProductEditViewModel model, string username)
        {
            if (model == null)
            {
                _logger.ErrorLog("Product not found","Edit Product","Product not found",username);
                return SystemSucceededTask.Failed("Product not found", string.Empty, false, true);
            }

            var products = await _products.Query().FirstAsync(x => x.Id == model.Id);
            products.Active = model.Active;
            products.BaseUnitId = model.BaseUnitId;
            products.CreatedOn = products.CreatedOn;
            products.Name = model.Name;
            products.Description = model.Description;
            products.ProductTypeId = model.ProductTypeId;
            try
            {
                await _products.SaveChangesAsync();
                _logger.InformationLog("Product " + model.Name + " updated successfully", "Edit Product", string.Empty,
                    username);
                return SystemSucceededTask.Success("Product " + model.Name + " updated successfully");
            }
            catch (SqlException ex)
            {
                _logger.ErrorLog(ex.Message,"Edit Product",ex.InnerException.Message,username);
                return SystemSucceededTask.Failed("Edit Product " + model.Name + " updated unsuccessfully",
                    ex.InnerException.Message, true, false);
            }
        }

        public  bool EnableDisableProduct(long id,string username)
        {

            var products = _products.Query().FirstOrDefaultAsync(x => x.Id == id).Result;
            if (products == null)
                return false;

            switch (products.Active)
            {
                case true:
                    products.Active = false;
                    break;
                case false:
                    products.Active = true;
                    break;
            }

            try
            {
                _location.SaveChanges();
                _logger.InformationLog("Enable/Disable product successfully", "Enable/Disable Product", string.Empty,
                    username);
                return true;
            }
            catch (SqlException ex)
            {
                _logger.ErrorLog(ex.Message,"Enable/Disable Product",ex.InnerException.Message,username);
                return false;
            }
        }

        public bool EnableDisableBaseUnit(long id,string username)
        {

            var baseUnit = _baseUnits.Query().FirstOrDefaultAsync(x => x.Id == id).Result;
            if (baseUnit == null)
                return false;

            switch (baseUnit.Active)
            {
                case true:
                    baseUnit.Active = false;
                    break;
                case false:
                    baseUnit.Active = true;
                    break;
            }

            try
            {
                _location.SaveChanges();
                _logger.InformationLog("Base Unit " + baseUnit.Name + " Enable/Disable successfully",
                    "Enable/Disable Base Unit", string.Empty, username);
                return true;
            }
            catch (SqlException ex)
            {
                _logger.ErrorLog(ex.Message,"Enable/Disable Base Unit",ex.InnerException.Message,username);
                return false;
            }
        }

        private bool ValidateProductCode(string code)
        {
            var exists = _products.Query().FirstOrDefault(x => x.Code == code);
            return exists == null;
        }

        public IEnumerable<Products> GetProducts()
        {
            return _products.Query();
        }

        public IEnumerable<Products> GetActiveProducts()
        {
            return _products.Query().Where(x => x.Active);
        }

        public Products GetProductByName(string name)
        {
            return _products.Query().FirstOrDefault(x => x.Name == name);
        }

        public Products GetProductByCode(string code)
        {
            return _products.Query().FirstOrDefault(x => x.Code == code);
        }
        
        public Products GetProductById(long id)
        {
            return _products.Query().FirstOrDefault(x => x.Id == id);
        }

        public IEnumerable<ProductList> GetProductList(long costCenterId,string productType)
        {
            var list = from cc in _costCenter.Query()
                join pd in _context.Set<CostCenterProductsDetails>() on cc.Id equals pd.CostCenterId
                join pr in _products.Query() on pd.ProductId equals pr.Id
                join cname in _costCenterProduct.Query() on pd.CostCenterProductsId equals cname.Id
                join bu in _baseUnits.Query() on pr.BaseUnitId equals bu.Id
                join pt in _productTypes.Query() on pr.ProductTypeId equals pt.Id 
                where cc.Id == costCenterId && cc.Active == true && pr.Active == true && pt.Type == productType
                select new ProductList
                {
                    Name = pr.Name,
                    BaseUnit = bu.Value,
                    Value = String.Empty,
                    CenterCostName = cc.Name
                };

            return list.ToList();
        }

        public IEnumerable<ProductListEdit> GetProductListEdit(long consumptionId)
        {
            var list = from cd in _consumptionDetails.Query().Where(x => x.ConsumptionId == consumptionId)
                join cc in _costCenter.Query() on cd.CostCenterId equals cc.Id
                join p in _products.Query() on cd.ProductId equals p.Id
                join c in _consumption.Query() on cd.ConsumptionId equals c.Id
                where c.FolioId == null
                       select new ProductListEdit
                {
                    CostCenterId = cc.Id,
                    CenterCostName = cc.Name,
                    Name = p.Name,
                    Value = cd.Weight.ToString(CultureInfo.CurrentCulture),
                    BaseUnit = cd.BaseUnit

                };
            return list.ToList();
        }

        public IEnumerable<ProductListEdit> GetProductListFolio(long consumptionId,long folio)
        {
            var list = from cd in _consumptionDetails.Query().Where(x => x.ConsumptionId == consumptionId)
                join cc in _costCenter.Query() on cd.CostCenterId equals cc.Id
                join p in _products.Query() on cd.ProductId equals p.Id
                join c in _consumption.Query() on cd.ConsumptionId equals c.Id
                where c.FolioId == folio
                select new ProductListEdit
                {
                    CostCenterId = cc.Id,
                    CenterCostName = cc.Name,
                    Name = p.Name,
                    Value = cd.Weight.ToString(CultureInfo.CurrentCulture),
                    BaseUnit = cd.BaseUnit

                };
            return list.ToList();
        }

        public IEnumerable<ProductList> GetProductListFailed(long costCenterId,string productType, string[] keys, string[] values)
        {
            var count = keys.Length;
            var productList= new List<ProductListFailed>();

            for (var i = 0; i < count; i++)
            {
                productList.Add(new ProductListFailed {Name = keys[i], Value = values[i]});
            }

            var list = from cc in _costCenter.Query()
                join pd in _context.Set<CostCenterProductsDetails>() on cc.Id equals pd.CostCenterId
                join pr in _products.Query() on pd.ProductId equals pr.Id
                join cname in _costCenterProduct.Query() on pd.CostCenterProductsId equals cname.Id
                join bu in _baseUnits.Query() on pr.BaseUnitId equals bu.Id
                join pl in productList on pr.Name equals pl.Name
                join pt in _productTypes.Query() on pr.ProductTypeId equals pt.Id
                where cc.Id == costCenterId && cc.Active == true && pr.Active == true && pt.Type == productType
                       select new ProductList
                {
                    Name = pr.Name,
                    BaseUnit = bu.Value,
                    Value = pl.Value,
                    CenterCostName = cc.Name
                };

            return list.ToList();
        }

        public IEnumerable<ProductListEdit> GetProductListEditFailed(long consumptionId,string[] keys, string[] values)
        {
            var count = keys.Length;
            var productList = new List<ProductListFailed>();

            for (var i = 0; i < count; i++)
            {
                productList.Add(new ProductListFailed { Name = keys[i], Value = values[i] });
            }

            var list = from cd in _consumptionDetails.Query().Where(x => x.ConsumptionId == consumptionId)
                join cc in _costCenter.Query() on cd.CostCenterId equals cc.Id
                join p in _products.Query() on cd.ProductId equals p.Id
                join pl in productList on p.Name equals pl.Name
                join c in _consumption.Query() on cd.ConsumptionId equals c.Id
                where c.FolioId == null
                       select new ProductListEdit
                {
                    CostCenterId = cc.Id,
                    CenterCostName = cc.Name,
                    Name = p.Name,
                    Value = pl.Value,
                    BaseUnit = cd.BaseUnit

                };
            return list.ToList();
        }

        public IList<LocationList> GetCostCenterList()
        {
            var locationList = new List<LocationList>();   
            var locList = (from a in GetActiveCostCenters()
                select new { LocationId = a.Id, LocationName = a.Name,Selected = false }).ToList();
            locationList.Add( new LocationList {LocationId = 0, LocationName = "Select a Cost Center",Selected = true});
            foreach (var item in locList)
            {
                locationList.Add(new LocationList
                {
                    LocationId = item.LocationId,
                    LocationName = item.LocationName,
                    Selected = false
                });
            }

            return locationList;
        }
        
        public IList<LocationList> GetCostCenterMealList()
        {
            var locationList = new List<LocationList>();   
            var locList = (from a in GetActiveCostCenters()
                where a.IsMeal == true
                select new { LocationId = a.Id, LocationName = a.Name,Selected = false }).ToList();
            locationList.Add( new LocationList {LocationId = 0, LocationName = "Select a Cost Center",Selected = true});
            foreach (var item in locList)
            {
                locationList.Add(new LocationList
                {
                    LocationId = item.LocationId,
                    LocationName = item.LocationName,
                    Selected = false
                });
            }

            return locationList;
        }
        
        public IList<LocationList> GetCostCenterMedicalList()
        {
            var locationList = new List<LocationList>();   
            var locList = (from a in GetActiveCostCenters()
                where a.IsMedical == true
                select new { LocationId = a.Id, LocationName = a.Name,Selected = false }).ToList();
            locationList.Add(new LocationList {LocationId = 0, LocationName = "Select a Cost Center", Selected = true});
            foreach (var item in locList)
            {
                locationList.Add(new LocationList
                {
                    LocationId = item.LocationId,
                    LocationName = item.LocationName,
                    Selected = false
                });
            }

            return locationList;
        }
       
        public async Task<bool> LoadInitialDataFarm()
        {
            //FarmInitialData
            var rootFolder = _hostingEnvironment.WebRootPath;
            var fileName = @"FarmInitial.xlsx";
            var path = Path.Combine(rootFolder, "FarmInitialData", fileName);
            try
            {
                using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read))
                {
                    var workbook = new XSSFWorkbook(fs);
                    //Load Locations
                    var sheet = workbook.GetSheetAt(workbook.GetSheetIndex("Location"));
                    var headerRow = sheet.GetRow(0);
                    var cellCount = headerRow.LastCellNum;
                    for (var i = (sheet.FirstRowNum +1); i <= sheet.LastRowNum; i++)
                    {
                        IRow row = sheet.GetRow(i);
                        if (row == null) continue;
                        if (row.Cells.All(d => d.CellType == CellType.Blank)) continue;
                        var location = new Locations();
                        for (int j = row.FirstCellNum; j < cellCount; j++)
                        {
                            
                            if (headerRow.GetCell(j).ToString() == "Name") location.Name = row.GetCell(j).ToString();
                            if (headerRow.GetCell(j).ToString() == "Code") location.Code = row.GetCell(j).ToString();
                            if (headerRow.GetCell(j).ToString() == "Description")
                                location.Description = row.GetCell(j).ToString();

                        }

                        location.Active = true;
                        location.CreatedOn = DateTimeOffset.Now;
                        _location.Add(location);
                        await _location.SaveChangesAsync();
                    }
                    //Load Cost Center
                    sheet = workbook.GetSheetAt(workbook.GetSheetIndex("CostCenter"));
                    headerRow = sheet.GetRow(0);
                    cellCount = headerRow.LastCellNum;
                    for (var i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++)
                    {
                        IRow row = sheet.GetRow(i);
                        if (row == null) continue;
                        if (row.Cells.All(d => d.CellType == CellType.Blank)) continue;
                        var costCenter = new CostCenter();
                        for (int j = row.FirstCellNum; j < cellCount; j++)
                        {

                            if (headerRow.GetCell(j).ToString() == "Name") costCenter.Name = row.GetCell(j).ToString();
                            if (headerRow.GetCell(j).ToString() == "Code") costCenter.Code = row.GetCell(j).ToString();
                            if (headerRow.GetCell(j).ToString() == "Description")
                                costCenter.Description = row.GetCell(j).ToString();
                            if (headerRow.GetCell(j).ToString() == "WarehouseCode")
                                costCenter.WarehouseCode = row.GetCell(j).ToString();
                            if (headerRow.GetCell(j).ToString() == "Location")
                                costCenter.LocationId = GetLocationByName(row.GetCell(j).ToString()).Id;
                            if (headerRow.GetCell(j).ToString() == "Meal")
                                costCenter.IsMeal = row.GetCell(j).ToString() == "1";
                            if (headerRow.GetCell(j).ToString() == "Medical")
                                costCenter.IsMedical = row.GetCell(j).ToString() == "1";
                            if (headerRow.GetCell(j).ToString() == "Farm")
                                costCenter.IsFarming = row.GetCell(j).ToString() == "1";


                        }

                        costCenter.Active = true;
                        costCenter.CreatedOn = DateTimeOffset.Now;
                        _costCenter.Add(costCenter);
                        await _costCenter.SaveChangesAsync();
                    }
                    //Load Base Unit
                    sheet = workbook.GetSheetAt(workbook.GetSheetIndex("BaseUnit"));
                    headerRow = sheet.GetRow(0);
                    cellCount = headerRow.LastCellNum;
                    for (var i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++)
                    {
                        IRow row = sheet.GetRow(i);
                        if (row == null) continue;
                        if (row.Cells.All(d => d.CellType == CellType.Blank)) continue;
                        var baseUnit = new BaseUnits();
                        for (int j = row.FirstCellNum; j < cellCount; j++)
                        {

                            if (headerRow.GetCell(j).ToString() == "Name") baseUnit.Name = row.GetCell(j).ToString();
                            if (headerRow.GetCell(j).ToString() == "Value") baseUnit.Value = row.GetCell(j).ToString();
                            if (headerRow.GetCell(j).ToString() == "Description")
                                baseUnit.Description = row.GetCell(j).ToString();

                        }

                        baseUnit.CreatedOn = DateTimeOffset.Now;
                        baseUnit.Active = true;
                        _baseUnits.Add(baseUnit);
                        await _baseUnits.SaveChangesAsync();
                    }
                    //Load Product
                    sheet = workbook.GetSheetAt(workbook.GetSheetIndex("Products"));
                    headerRow = sheet.GetRow(0);
                    cellCount = headerRow.LastCellNum;
                    for (var i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++)
                    {
                        IRow row = sheet.GetRow(i);
                        if (row == null) continue;
                        if (row.Cells.All(d => d.CellType == CellType.Blank)) continue;
                        var products = new Products();
                        for (int j = row.FirstCellNum; j < cellCount; j++)
                        {

                            if (headerRow.GetCell(j).ToString() == "Name") products.Name = row.GetCell(j).ToString();
                            if (headerRow.GetCell(j).ToString() == "Code") products.Code = row.GetCell(j).ToString();
                            if (headerRow.GetCell(j).ToString() == "Description")
                                products.Description = row.GetCell(j).ToString();
                            if (headerRow.GetCell(j).ToString() == "BaseUnit")
                                products.BaseUnitId = GetBaseUnitByValue(row.GetCell(j).ToString()).Id;

                        }
                        products.Active = true;
                        products.CreatedOn = DateTimeOffset.Now;
                        _products.Add(products);
                        await _products.SaveChangesAsync();
                    }
                    //Assign Products to Cost Center
                    sheet = workbook.GetSheetAt(workbook.GetSheetIndex("CostCenterProducts"));
                    headerRow = sheet.GetRow(0);
                    cellCount = headerRow.LastCellNum;
                    for (var i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++)
                    {
                        IRow row = sheet.GetRow(i);
                        if (row == null) continue;
                        if (row.Cells.All(d => d.CellType == CellType.Blank)) continue;
                        var costCenterProductsDetails = new CostCenterProductsDetails();
                        for (int j = row.FirstCellNum; j < cellCount; j++)
                        {

                            if (headerRow.GetCell(j).ToString() == "CostCenterName")
                            {
                                if (_costCenterProduct.Query().FirstOrDefault(x => x.Name == "Productos para " 
                                                                                   + row.GetCell(j)) == null)
                                {
                                    var costCenterProducts = new CostCenterProducts()
                                    {
                                        Name = "Productos para " + row.GetCell(j),
                                        Active = true,
                                        CreatedOn = DateTimeOffset.Now
                                    };
                                    _costCenterProduct.Add(costCenterProducts);
                                    await _costCenterProduct.SaveChangesAsync();
                                    costCenterProductsDetails.CostCenterProductsId = costCenterProducts.Id;
                                }
                                else
                                {
                                    costCenterProductsDetails.CostCenterProductsId = _costCenterProduct.Query()
                                        .FirstOrDefault(x => x.Name == "Productos para " + row.GetCell(j)).Id;
                                }
                            }

                            if (headerRow.GetCell(j).ToString() == "CostCenterName")
                                costCenterProductsDetails.CostCenterId =
                                    GetCostCenterByName(row.GetCell(j).ToString()).Id;
                            if (headerRow.GetCell(j).ToString() == "ProductCode")
                                costCenterProductsDetails.ProductId = GetProductByCode(row.GetCell(j).ToString()).Id;

                        }
                        _context.Set<CostCenterProductsDetails>().Add(costCenterProductsDetails);
                        await _context.SaveChangesAsync();
                    }
                }
                _logger.InformationLog("Load Initial Success", "Load Initial", string.Empty, string.Empty);
				return true;
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex.Message, "Load Initial", ex.InnerException.Message, string.Empty);
				return false;
            }

        }
        /*
        private IList<DateFolio> SetFolio(IList<DateList> dateList,int folio)
        {
            var dateFolio = new List<DateFolio>();
            foreach (var item in dateList)
            {
                dateFolio.Add(new DateFolio(){Date = item.Date, Folio = folio});
                folio++;
            }

            return dateFolio;
        }
        */

		public IList<ConsumptionReport> ConsumptionReport(DateTime fromReportDate,DateTime toReportDate,string warehouseName,bool? setFolio,long folio = -1 )
		{
            var query =
		        from cd in _consumptionDetails.Query()
		        join pr in _products.Query() on cd.ProductId equals pr.Id
		        join cc in _costCenter.Query() on cd.CostCenterId equals cc.Id
				join cp in _consumption.Query() on cd.ConsumptionId equals cp.Id
                where Int32.Parse(cp.LateCreatedOn.ToString("yyyyMMdd")) >= Int32.Parse(fromReportDate.ToString("yyyyMMdd"))
                      && Int32.Parse(cp.LateCreatedOn.ToString("yyyyMMdd")) <= Int32.Parse(toReportDate.ToString("yyyyMMdd"))
                      && cc.WarehouseCode == warehouseName
                      && cp.FolioId == null
                orderby cp.LateCreatedOn.ToString("yyyyMMdd")
                select new ConsumptionReport
                {
                    Warehouse = cd.WarehouseCode,
                    Folio = 0,
                    GeneratedDate = cp.LateCreatedOn.ToString("dd/MM/yyyy"), //toReportDate.ToString("dd/MM/yyyy"),
                    CenterCostCode = cc.Code,
                    ProductCode = pr.Code,
                    BaseUnit = cd.BaseUnit,
                    Amount = cd.Weight,
                    ConsumptionId = cp.Id
                    
                };


		    if (!query.Any() && folio ==-1)
		    {
		        var nullReport = new List<ConsumptionReport>();
		        return nullReport;
		    }

		    //var folioDates = SetFolio(dates.ToList(), folio);
            var setFolioReport =
                from q in query
                //join fd in folioDates on q.GeneratedDate equals fd.Date
                select new ConsumptionReport
                {
                    Warehouse = q.Warehouse,
                    Folio = 0,
                    GeneratedDate = q.GeneratedDate,
                    CenterCostCode = q.CenterCostCode,
                    ProductCode = q.ProductCode,
                    BaseUnit = q.BaseUnit,
                    Amount = q.Amount,
                    ConsumptionId = q.ConsumptionId
                };

		    var setFolioReportCount = setFolioReport.Count();
            //TODO:Save Into Consumption Folio Table
            //var res = CreateConsumptionFolio(setFolioReport.ToList(),fromReportDate,toReportDate).Result;

            var report = setFolioReport
                .GroupBy(g => new
		        {
                    g.Warehouse,
                    g.Folio,
                    g.GeneratedDate,
                    g.WarehouseOut,
                    g.Description,
                    g.CenterCostCode,
                    g.ProductCode,
                    g.BaseUnit,
                    //g.ConsumptionId

		        })
		        .Select(x => new ConsumptionReport
                {
                    Warehouse = x.Key.Warehouse,
                    Folio = x.Key.Folio,
                    GeneratedDate = x.Key.GeneratedDate,
                    WarehouseOut = x.Key.WarehouseOut,
                    Description = x.Key.Description,
                    CenterCostCode = x.Key.CenterCostCode,
                    ProductCode = x.Key.ProductCode,
                    BaseUnit = x.Key.BaseUnit,
		            Amount = x.Sum(o => o.Amount),
                    //ConsumptionId = x.Key.ConsumptionId
		        });


            
            if (setFolioReport.Any() && setFolio == true)
            {
                folio = SaveFolio(fromReportDate, toReportDate, setFolioReport.ToList());
            }

		    if (folio != -1)
		    {
		        var final =
		            from cd in _consumptionDetails.Query()
		            join pr in _products.Query() on cd.ProductId equals pr.Id
		            join cc in _costCenter.Query() on cd.CostCenterId equals cc.Id
		            join cp in _consumption.Query().Where(x => x.FolioId == folio) on cd.ConsumptionId equals cp.Id
		            orderby cp.LateCreatedOn.ToString("yyyyMMdd")
		            select new ConsumptionReport
		            {
		                Warehouse = cd.WarehouseCode,
		                Folio = 0,
		                GeneratedDate = cp.LateCreatedOn.ToString("dd/MM/yyyy"), //toReportDate.ToString("dd/MM/yyyy"),
		                CenterCostCode = cc.Code,
		                ProductCode = pr.Code,
		                BaseUnit = cd.BaseUnit,
		                Amount = cd.Weight,
		                ConsumptionId = cp.Id

		            };
		        var report2 = final
                    .GroupBy(g => new
		            {
		                g.Warehouse,
		                g.Folio,
		                g.GeneratedDate,
		                g.WarehouseOut,
		                g.Description,
		                g.CenterCostCode,
		                g.ProductCode,
		                g.BaseUnit,
		                //g.ConsumptionId

		            })
		            .Select(x => new ConsumptionReport
		            {
		                Warehouse = x.Key.Warehouse,
		                Folio = x.Key.Folio,
		                GeneratedDate = x.Key.GeneratedDate,
		                WarehouseOut = x.Key.WarehouseOut,
		                Description = x.Key.Description,
		                CenterCostCode = x.Key.CenterCostCode,
		                ProductCode = x.Key.ProductCode,
		                BaseUnit = x.Key.BaseUnit,
		                Amount = x.Sum(o => o.Amount),
		                //ConsumptionId = x.Key.ConsumptionId
		            });
		        return report2.ToList();
		    }
            return report.ToList();
		    
        }

        private long SaveFolio(DateTime fromReportDate, DateTime toReportDate, IList<ConsumptionReport> report)
        {
            var folio = new Folio
            {
                FinalDate = toReportDate,
                InitialDate = fromReportDate,
                IsPreviewFolio = false
            };
            _folio.Add(folio);
            _folio.SaveChanges();

            var consumptions = report.Select(x => x.ConsumptionId).Distinct().ToList();

            foreach (var item in consumptions)
            {
                var consumption = _consumption.Query().FirstOrDefault(x => x.Id == item);
                consumption.FolioId = folio.Id;
                _consumption.SaveChanges();
            }

            return folio.Id;
        }


        public IList<ConsumptionReport> GeConsumptionReportById(long folio)
        {
            var final =
                    from cd in _consumptionDetails.Query()
                    join pr in _products.Query() on cd.ProductId equals pr.Id
                    join cc in _costCenter.Query() on cd.CostCenterId equals cc.Id
                    join cp in _consumption.Query() on cd.ConsumptionId equals cp.Id
                    join f in _folio.Query() on cp.FolioId equals f.Id
                    where cp.FolioId == folio
                    orderby cp.LateCreatedOn.ToString("yyyyMMdd")
                    select new ConsumptionReport
                    {
                        Warehouse = cd.WarehouseCode,
                        Folio = 0,
                        GeneratedDate = cp.LateCreatedOn.ToString("dd/MM/yyyy"), //toReportDate.ToString("dd/MM/yyyy"),
                        CenterCostCode = cc.Code,
                        ProductCode = pr.Code,
                        BaseUnit = cd.BaseUnit,
                        Amount = cd.Weight,
                        ConsumptionId = cp.Id

                    };
            var report2 = final
                .GroupBy(g => new
                {
                    g.Warehouse,
                    g.Folio,
                    g.GeneratedDate,
                    g.WarehouseOut,
                    g.Description,
                    g.CenterCostCode,
                    g.ProductCode,
                    g.BaseUnit,
                        //g.ConsumptionId

                    })
                .Select(x => new ConsumptionReport
                {
                    Warehouse = x.Key.Warehouse,
                    Folio = x.Key.Folio,
                    GeneratedDate = x.Key.GeneratedDate,
                    WarehouseOut = x.Key.WarehouseOut,
                    Description = x.Key.Description,
                    CenterCostCode = x.Key.CenterCostCode,
                    ProductCode = x.Key.ProductCode,
                    BaseUnit = x.Key.BaseUnit,
                    Amount = x.Sum(o => o.Amount),
                        //ConsumptionId = x.Key.ConsumptionId
                    });
            return report2.ToList();
        }

       
        
        public IList<WarehouseList> GetWarehouseListNames()
        {
            var warehouseList = new List<WarehouseList>();
            var warehouseNames = from cc in _costCenter.Query()
                select new { WarehouseName = cc.WarehouseCode,Selected = false };
            var report = warehouseNames.GroupBy(o => o.WarehouseName).Select(x => x.First()).ToList();
            warehouseList.Add(new WarehouseList {WarehouseName = "Select a Warehouse",Selected = true});
            foreach (var item in report)
            {
                warehouseList.Add(new WarehouseList {WarehouseName = item.WarehouseName,Selected = item.Selected});
            }

            return warehouseList;

        }

        public IList<Folio> GetFolio()
        {
            return _folio.Query().ToList();
        }

        public IList<ConsumptionList> GetConsumptionListByFolio(long folio)
        {
            var date = DateTimeOffset.Now;

            var query = from c in _consumption.Query().Where(x => x.CreatedOn >= date.AddDays(-10000))
                join cd in _consumptionDetails.Query() on c.Id equals cd.ConsumptionId
                join cc in _costCenter.Query() on cd.CostCenterId equals cc.Id
                where c.FolioId == folio
                select new ConsumptionList
                {
                    Id = c.Id,
                    CenterCost = cc.Name,
                    ConsumptionEffectiveEntryDate = c.LateCreatedOn.ToLocalTime().ToString("yyyy/MM/dd HH:mm:ss"),
                    ConsumptionEntryDate = c.CreatedOn.ToLocalTime().ToString("yyyy/MM/dd HH:mm:ss "),
                    ConsumptionName = c.Name,
                    Warehouse = cd.WarehouseCode
                };
            var list = query
                .GroupBy(g => new
                {
                    g.Id,
                    g.CenterCost,
                    g.ConsumptionEffectiveEntryDate,
                    g.ConsumptionEntryDate,
                    g.ConsumptionName,
                    g.Warehouse
                })
                .Select(x => new ConsumptionList
                {
                    Id = x.Key.Id,
                    CenterCost = x.Key.CenterCost,
                    ConsumptionEffectiveEntryDate = x.Key.ConsumptionEffectiveEntryDate,
                    ConsumptionEntryDate = x.Key.ConsumptionEntryDate,
                    ConsumptionName = x.Key.ConsumptionName,
                    Warehouse = x.Key.Warehouse
                }).ToList();

            return list;
        }

        public IList<ConsumptionList> GetConsumptionList()
        {
            var date = DateTimeOffset.Now;   
                
            var query = from c in _consumption.Query().Where(x => x.CreatedOn >= date.AddDays(-10000))
                join cd in _consumptionDetails.Query() on c.Id equals cd.ConsumptionId
                join cc in _costCenter.Query() on cd.CostCenterId equals cc.Id
                where c.FolioId == null
                select new ConsumptionList
                {
                    Id = c.Id,
                    CenterCost = cc.Name,
                    ConsumptionEffectiveEntryDate = c.LateCreatedOn.ToLocalTime().ToString("yyyy/MM/dd HH:mm:ss"),
                    ConsumptionEntryDate = c.CreatedOn.ToLocalTime().ToString("yyyy/MM/dd HH:mm:ss "),
                    ConsumptionName = c.Name,
                    Warehouse = cd.WarehouseCode
                };
            var list = query
                .GroupBy(g => new
                {
                    g.Id,
                    g.CenterCost,
                    g.ConsumptionEffectiveEntryDate,
                    g.ConsumptionEntryDate,
                    g.ConsumptionName,
                    g.Warehouse
                })
                .Select(x => new ConsumptionList
                {
                    Id = x.Key.Id,
                    CenterCost = x.Key.CenterCost,
                    ConsumptionEffectiveEntryDate = x.Key.ConsumptionEffectiveEntryDate,
                    ConsumptionEntryDate = x.Key.ConsumptionEntryDate,
                    ConsumptionName = x.Key.ConsumptionName,
                    Warehouse = x.Key.Warehouse
                }).ToList();

            return list;
        }

        public Consumptions GetConsumptionById(long id)
        {
            return _consumption.Query().FirstOrDefault(x => x.Id == id);
        }

        public IList<ConsumptionDetails> ConsumptionDetailsByConsumptionId(long id)
        {
            return _consumptionDetails.Query().Where(x => x.ConsumptionId == id).ToList();
        }
        
    }
}
