﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using AutoMapper;
using H.Core.Calculators.Carbon;
using H.Core.Calculators.Economics;
using H.Core.Calculators.Nitrogen;
using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Models.Animals;
using H.Core.Models.Animals.Dairy;
using H.Core.Models.LandManagement.Fields;
using H.Core.Providers.Animals;
using H.Core.Providers.Carbon;
using H.Core.Providers.Climate;
using H.Core.Providers.Economics;
using H.Core.Providers.Energy;
using H.Core.Providers.Fertilizer;
using H.Core.Providers.Irrigation;
using H.Core.Providers.Nitrogen;
using H.Core.Providers.Plants;
using H.Core.Providers.Soil;
using H.Core.Services.Initialization.Animals;
using H.Core.Services.Initialization.Crops;
using H.Core.Services.LandManagement;
using H.Infrastructure;

namespace H.Core.Services.Initialization
{
    public partial class InitializationService :  IInitializationService
    {
        #region Fields

        private readonly ICropInitializationService _cropInitializationService;
        private readonly IAnimalInitializationService _animalInitializationService;

        #endregion

        #region Constructors

        public InitializationService()
        {
            _cropInitializationService = new CropInitializationService();
            _animalInitializationService =new AnimalInitializationService();
        }
        
        #endregion

        #region Public Methods

        public void CheckInitialization(Farm farm)
        {
            if (farm == null)
            {
                return;
            }

            if (farm.DefaultSoilData == null)
            {
                return;
            }

            if (farm.ClimateData == null)
            {
                return;
            }

            var climateData = farm.ClimateData;

            var barnTemperature = climateData.BarnTemperatureData;
            if (barnTemperature == null || barnTemperature.IsInitialized == false)
            {
                this.InitializeBarnTemperature(farm);
            }
        }

        public void ReInitializeFarms(IEnumerable<Farm> farms)
        {
            foreach (var farm in farms)
            {
                // Defaults (carbon concentration)
                this.InitializeCarbonConcentration(farm);

                // Nitrogen Fixation
                _cropInitializationService.InitializeNitrogenFixation(farm);

                // Table 4
                this.InitializeIrrigationWaterApplication(farm);

                // Table 6
                this.InitializeManureCompositionData(farm);

                // Table 17
                this.InitializeCattleFeedingActivity(farm);

                // Table 21
                this.InitializeMilkProduction(farm);

                // Table 22
                this.InitializeLivestockCoefficientSheep(farm);

                // Table 27
                this.InitializeAnnualEntericMethaneRate(farm);

                // Table 29
                this.InitializeManureExcretionRate(farm);

                // Table 31
                this.InitializeVolatileSolidsExcretion(farm);

                // Table 35
                this.InitializeMethaneProducingCapacity(farm);

                // Table 36
                this.InitializeDefaultEmissionFactors(farm);

                //Table 38
                this.InitializeOtherLivestockDefaultCH4EmissionFactor(farm);

                // Table 44
                this.InitializeManureMineralizationFractions(farm);

                // Table 50
                this.InitializeFuelEnergy(farm);

                // Table 51
                this.InitializeHerbicideEnergy(farm);

                // Table 63
                this.InitializeBarnTemperature(farm);
            }
        }


        public void InitializeParameterAdjustmenstForManure(Farm farm)
        {
            if (farm != null)
            {
                foreach (var animalComponent in farm.AnimalComponents)
                {
                    foreach (var animalGroup in animalComponent.Groups)
                    {
                        foreach (var managementPeriod in animalGroup.ManagementPeriods)
                        {


                        }
                    }
                }
            }
        }

        public void InitializeCarbonConcentration(Farm farm)
        {
            _cropInitializationService.InitializeCarbonConcentration(farm);
        }

        public void InitializeCarbonConcentration(CropViewItem viewItem, Defaults defaults)
        {
            _cropInitializationService.InitializeCarbonConcentration(viewItem, defaults);
        }

        public void InitializeBiomassCoefficients(CropViewItem viewItem, Farm farm)
        {
            _cropInitializationService.InitializeBiomassCoefficients(viewItem, farm);
        }

        public void InitializeLumCMaxValues(CropViewItem cropViewItem, Farm farm)
        {
            _cropInitializationService.InitializeLumCMaxValues(cropViewItem, farm);
        }

        public void InitializePercentageReturns(Farm farm, CropViewItem viewItem)
        {
            _cropInitializationService.InitializePercentageReturns(farm, viewItem);
        }

        public void InitializeYield(CropViewItem viewItem, Farm farm)
        {
            _cropInitializationService.InitializeYield(viewItem, farm);
        }

        public void InitializeSilageCropYield(CropViewItem silageCropViewItem, Farm farm)
        {
            _cropInitializationService.InitializeSilageCropYield(silageCropViewItem, farm);
        }

        public void InitializeYieldForAllYears(IEnumerable<CropViewItem> cropViewItems, Farm farm, FieldSystemComponent fieldSystemComponent)
        {
            _cropInitializationService.InitializeYieldForAllYears(cropViewItems, farm, fieldSystemComponent);
        }

        public void InitializeYieldForYear(Farm farm, CropViewItem viewItem, FieldSystemComponent fieldSystemComponent)
        {
            _cropInitializationService.InitializeYieldForYear(farm, viewItem, fieldSystemComponent);
        }

        public void InitializeEconomicDefaults(CropViewItem cropViewItem, Farm farm)
        {
            _cropInitializationService.InitializeEconomicDefaults(cropViewItem, farm);
        }

        public double CalculateAmountOfProductRequired(Farm farm, CropViewItem viewItem,
            FertilizerApplicationViewItem fertilizerApplicationViewItem)
        {
            return _cropInitializationService.CalculateAmountOfProductRequired(farm, viewItem, fertilizerApplicationViewItem);
        }

        public void InitializePhosphorusFertilizerRate(CropViewItem viewItem, Farm farm)
        {
            _cropInitializationService.InitializePhosphorusFertilizerRate(viewItem, farm);
        }

        public void InitializeBlendData(FertilizerApplicationViewItem fertilizerApplicationViewItem)
        {
            _cropInitializationService.InitializeBlendData(fertilizerApplicationViewItem);
        }

        public double CalculateRequiredNitrogenFertilizer(Farm farm, CropViewItem viewItem,
            FertilizerApplicationViewItem fertilizerApplicationViewItem)
        {
            return _cropInitializationService.CalculateRequiredNitrogenFertilizer(farm, viewItem, fertilizerApplicationViewItem);
        }

        public void InitializeNitrogenContent(CropViewItem viewItem, Farm farm)
        {
            _cropInitializationService.InitializeNitrogenContent(viewItem, farm);
        }

        public void InitializeNitrogenFixation(CropViewItem viewItem)
        {
            _cropInitializationService.InitializeNitrogenFixation(viewItem);
        }

        public void InitializeNitrogenFixation(Farm farm)
        {
            _cropInitializationService.InitializeNitrogenFixation(farm);
        }

        public void InitializeHerbicideEnergy(Farm farm)
        {
            _cropInitializationService.InitializeHerbicideEnergy(farm);
        }

        public void InitializeHerbicideEnergy(Farm farm, CropViewItem viewItem)
        {
            _cropInitializationService.InitializeHerbicideEnergy(farm, viewItem);
        }

        public void InitializeIrrigationWaterApplication(Farm farm)
        {
            _cropInitializationService.InitializeIrrigationWaterApplication(farm);
        }

        public void InitializeIrrigationWaterApplication(Farm farm, CropViewItem viewItem)
        {
            _cropInitializationService.InitializeIrrigationWaterApplication(farm, viewItem);
        }

        public void InitializeMoistureContent(Farm farm)
        {
            _cropInitializationService.InitializeMoistureContent(farm);
        }

        public void InitializeMoistureContent(Table_7_Relative_Biomass_Information_Data residueData, CropViewItem cropViewItem)
        {
            _cropInitializationService.InitializeMoistureContent(residueData, cropViewItem);
        }

        public void InitializeMoistureContent(CropViewItem viewItem, Farm farm)
        {
            _cropInitializationService.InitializeMoistureContent(viewItem, farm);
        }

        public void InitializeSoilProperties(CropViewItem viewItem, Farm farm)
        {
            _cropInitializationService.InitializeSoilProperties(viewItem, farm);
        }

        public void InitializeFuelEnergy(Farm farm)
        {
            _cropInitializationService.InitializeFuelEnergy(farm);
        }

        public void InitializeFuelEnergy(Farm farm, CropViewItem viewItem)
        {
            _cropInitializationService.InitializeFuelEnergy(farm, viewItem);
        }

        public void InitializeHarvestMethod(CropViewItem viewItem, Farm farm)
        {
            _cropInitializationService.InitializeHarvestMethod(viewItem, farm);
        }

        public void InitializeFallow(CropViewItem viewItem, Farm farm)
        {
            _cropInitializationService.InitializeFallow(viewItem, farm);
        }

        public void InitializeTillageType(CropViewItem viewItem, Farm farm)
        {
            _cropInitializationService.InitializeTillageType(viewItem, farm);
        }

        public void InitializeLigninContent(CropViewItem cropViewItem, Farm farm)
        {
            _cropInitializationService.InitializeLigninContent(cropViewItem, farm);
        }

        public void InitializeUserDefaults(CropViewItem viewItem, GlobalSettings globalSettings)
        {
            _cropInitializationService.InitializeUserDefaults(viewItem, globalSettings);
        }

        public void InitializePerennialDefaults(CropViewItem viewItem, Farm farm)
        {
            _cropInitializationService.InitializePerennialDefaults(viewItem, farm);
        }

        public void InitializeCropDefaults(CropViewItem viewItem, Farm farm, GlobalSettings globalSettings)
        {
            _cropInitializationService.InitializeCropDefaults(viewItem, farm, globalSettings);
        }

        public void InitializeCropDefaults(Farm farm, GlobalSettings globalSettings)
        {
            _cropInitializationService.InitializeCropDefaults(farm, globalSettings);
        }

        public void InitializeOtherLivestockDefaultCH4EmissionFactor(Farm farm)
        {
            _animalInitializationService.InitializeOtherLivestockDefaultCH4EmissionFactor(farm);
        }

        public void InitializeVolatileSolidsExcretion(Farm farm)
        {
            _animalInitializationService.InitializeVolatileSolidsExcretion(farm);
        }

        public void InitializeVolatileSolidsExcretion(ManagementPeriod managementPeriod, Province province)
        {
            _animalInitializationService.InitializeVolatileSolidsExcretion(managementPeriod, province);
        }

        public void InitializeAnnualEntericMethaneRate(Farm farm)
        {
            _animalInitializationService.InitializeAnnualEntericMethaneRate(farm);
        }

        public void InitializeAnnualEntericMethaneRate(ManagementPeriod managementPeriod)
        {
            _animalInitializationService.InitializeAnnualEntericMethaneRate(managementPeriod);
        }

        public void InitializeMethaneProducingCapacity(Farm farm)
        {
            _animalInitializationService.InitializeMethaneProducingCapacity(farm);
        }

        public void InitializeMethaneProducingCapacity(ManagementPeriod managementPeriod)
        {
            _animalInitializationService.InitializeMethaneProducingCapacity(managementPeriod);
        }

        public void ReinitializeBeddingMaterial(Farm farm)
        {
            _animalInitializationService.ReinitializeBeddingMaterial(farm);
        }

        public void InitializeBeddingMaterial(ManagementPeriod managementPeriod,
            Table_30_Default_Bedding_Material_Composition_Data data)
        {
            _animalInitializationService.InitializeBeddingMaterial(managementPeriod, data);
        }

        public void InitializeManureMineralizationFractions(Farm farm)
        {
            _animalInitializationService.InitializeManureMineralizationFractions(farm);
        }

        public void InitializeManureMineralizationFractions(ManagementPeriod managementPeriod)
        {
            _animalInitializationService.InitializeManureMineralizationFractions(managementPeriod);
        }

        public void InitializeManureExcretionRate(Farm farm)
        {
            _animalInitializationService.InitializeManureExcretionRate(farm);
        }

        public void InitializeManureExcretionRate(ManagementPeriod managementPeriod)
        {
            _animalInitializationService.InitializeManureExcretionRate(managementPeriod);
        }

        public void InitializeBarnTemperature(Farm farm)
        {
            _animalInitializationService.InitializeBarnTemperature(farm);
        }

        public void InitializeMilkProduction(Farm farm)
        {
            _animalInitializationService.InitializeMilkProduction(farm);
        }

        public void InitializeManureCompositionData(Farm farm)
        {
            _animalInitializationService.InitializeManureCompositionData(farm);
        }

        public void InitializeManureCompositionData(ManagementPeriod managementPeriod,
            DefaultManureCompositionData manureCompositionData)
        {
            _animalInitializationService.InitializeManureCompositionData(managementPeriod, manureCompositionData);
        }

        public void InitializeLivestockCoefficientSheep(Farm farm)
        {
            _animalInitializationService.InitializeLivestockCoefficientSheep(farm);
        }

        public void InitializeDefaultEmissionFactors(Farm farm)
        {
            _animalInitializationService.InitializeDefaultEmissionFactors(farm);
        }

        public void InitializeDefaultEmissionFactors(Farm farm, ManagementPeriod managementPeriod)
        {
            _animalInitializationService.InitializeDefaultEmissionFactors(farm, managementPeriod);
        }

        public void InitializeCattleFeedingActivity(Farm farm)
        {
            _animalInitializationService.InitializeCattleFeedingActivity(farm);
        }

        public void InitializeFeedingActivityCoefficient(ManagementPeriod managementPeriod)
        {
            _animalInitializationService.InitializeFeedingActivityCoefficient(managementPeriod);
        }

        public void InitializeMilkProduction(ManagementPeriod managementPeriod, SoilData soilData)
        {
            _animalInitializationService.InitializeMilkProduction(managementPeriod, soilData);
        }

        public void InitializeLeachingFraction(Farm farm, ManagementPeriod managementPeriod)
        {
            _animalInitializationService.InitializeLeachingFraction(farm, managementPeriod);
        }

        public void InitializeLeachingFraction(Farm farm)
        {
            _animalInitializationService.InitializeLeachingFraction(farm);
        }

        #endregion
    }
}