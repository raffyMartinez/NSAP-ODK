using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using NSAP_ODK.Utilities;

namespace NSAP_ODK.NSAPMysql
{
    public static class CreateMySQLTables
    {

        public static int CreateTables()
        {
            int tableCount = 0;
            using (var conn = new MySqlConnection(MySQLConnect.ConnectionString()))
            {

                using (var cmd = conn.CreateCommand())
                {
                    //v_unload_1 28
                    conn.Open();
                    #region FMA
                    cmd.CommandText = @"CREATE TABLE IF NOT EXISTS `fma`(
                                        `fma_id` INT NOT NULL,
                                        `fma_name` VARCHAR(40) NOT NULL,
                                        PRIMARY KEY (`fma_id` ASC) VISIBLE )
                                        COMMENT='Fishery management areas or FMAs'
                                        ENGINE=InnoDB;";

                    try
                    {
                        cmd.ExecuteNonQuery();
                        tableCount++;
                    }
                    catch (MySqlException msex)
                    {

                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex);
                    }
                    #endregion

                    #region NSAP Region
                    cmd.CommandText = @" CREATE TABLE IF NOT EXISTS `nsap_region`(
                                    `code` VARCHAR(6) NOT NULL,
                                    `region_name` VARCHAR(50) NOT NULL,
                                    `short_name` VARCHAR(30) NOT NULL,
                                    `sequence` INT NOT NULL,
                                    PRIMARY KEY (`code` ASC) VISIBLE )
                                    COMMENT ='Philippines administrative regions'
                                    ENGINE=InnoDB;";
                    try
                    {
                        cmd.ExecuteNonQuery();
                        tableCount++;
                    }
                    catch (MySqlException msex)
                    {

                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex);
                    }
                    #endregion

                    #region NSAP Region FMA
                    cmd.CommandText = @"CREATE TABLE IF NOT EXISTS `nsap_region_fma` (
                                        `nsap_region` VARCHAR(6) NOT NULL,
                                        `fma` INT NOT NULL,
                                        `row_id` INT NOT NULL,
                                        PRIMARY KEY (`row_id`),
                                        INDEX `nsap_region_nrf_fk_idx` (`nsap_region` ASC) VISIBLE,
                                        INDEX `fma_nrf_fk_idx` (`fma` ASC) VISIBLE,
                                        UNIQUE KEY `altKey_nrf` (`nsap_region`,`fma`),
                                        CONSTRAINT `nsap_region_nrf_fk`
                                          FOREIGN KEY (`nsap_region`)
                                          REFERENCES `nsap_region` (`code`)
                                          ON DELETE NO ACTION
                                          ON UPDATE NO ACTION )
                                        COMMENT = 'FMAs in administrative regions'
                                        ENGINE=InnoDB;";

                    try
                    {
                        cmd.ExecuteNonQuery();
                        tableCount++;
                    }
                    catch (MySqlException msex)
                    {

                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex);
                    }
                    #endregion

                    #region Enumerators
                    cmd.CommandText = @" CREATE TABLE IF NOT EXISTS `nsap_enumerators`(
                                    `enumerator_id` INT NOT NULL,
                                    `enumerator_name` VARCHAR(40) NOT NULL,
                                    PRIMARY KEY (`enumerator_id` ASC) VISIBLE )
                                    COMMENT='NSAP enumerators'
                                    ENGINE=InnoDB;";
                    try
                    {
                        cmd.ExecuteNonQuery();
                        tableCount++;
                    }
                    catch (MySqlException msex)
                    {

                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex);
                    }
                    #endregion

                    #region NSAP Region enumerator
                    cmd.CommandText = @"CREATE TABLE IF NOT EXISTS `nsap_region_enumerator`(
                                    `row_id` INT NOT NULL,
                                    `enumerator_id` INT NOT NULL,
                                    `region` VARCHAR(6) NOT NULL,
                                    `date_start` DATE NOT NULL,
                                    `date_end` DATE NULL,
                                    `date_first_sampling` DATE NULL,
                                    PRIMARY KEY (`row_id`),
                                    INDEX `enumerator_id_nre_fk_idx` (`enumerator_id` ASC) VISIBLE,
                                    INDEX `region_nre_fk_idx` (`region` ASC) VISIBLE,
                                    UNIQUE KEY `altKey_nre` (`enumerator_id`,`region`),
                                    CONSTRAINT `enumerator_id_nre_fk`
                                      FOREIGN KEY (`enumerator_id`)
                                      REFERENCES `nsap_enumerators` (`enumerator_id`)
                                      ON DELETE NO ACTION
                                      ON UPDATE NO ACTION,
                                    CONSTRAINT `region_nre_fk`
                                      FOREIGN KEY (`region`)
                                      REFERENCES `nsap_region` (`code`)
                                      ON DELETE NO ACTION
                                      ON UPDATE NO ACTION )
                                    COMMENT='Enumerators in a region'
                                    ENGINE=InnoDB";
                    try
                    {
                        cmd.ExecuteNonQuery();
                        tableCount++;
                    }
                    catch (MySqlException msex)
                    {

                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex);
                    }
                    #endregion

                    #region Provinces
                    cmd.CommandText = @"CREATE TABLE IF NOT EXISTS `provinces`(
                                        `prov_no` INT NOT NULL,
                                        `province_name` VARCHAR(40) NOT NULL,
                                        `nsap_region` VARCHAR(6) NOT NULL,
                                        PRIMARY KEY (`prov_no` ASC) VISIBLE,
                                        UNIQUE INDEX `province_name_idx` (`province_name` ASC) VISIBLE,
                                        INDEX `nsap_region_p_fk_idx`(`nsap_region` ASC) VISIBLE,
                                        CONSTRAINT `nsap_region_p_fk`
                                          FOREIGN KEY (`nsap_region`)
                                          REFERENCES `nsap_region`(`code`)
                                          ON DELETE NO ACTION
                                          ON UPDATE NO ACTION )
                                        COMMENT='Philippines provinces'
                                        ENGINE=InnoDB;";

                    try
                    {
                        cmd.ExecuteNonQuery();
                        tableCount++;
                    }
                    catch (MySqlException msex)
                    {

                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex);
                    }
                    #endregion

                    #region Municipalities
                    cmd.CommandText = @"CREATE TABLE IF NOT EXISTS `municipalities`(
                                    `mun_no` INT NOT NULL,
                                    `prov_no` INT NOT NULL,
                                    `municipality` VARCHAR(40) NOT NULL,
                                    `y_coord` DOUBLE NULL,
                                    `x_coord` DOUBLE NULL,
                                    `is_coastal` TINYINT(1) NOT NULL,
                                    PRIMARY KEY (`mun_no` ASC) VISIBLE,
                                    INDEX `prov_no_m_fk_idx`(`prov_no` ASC) VISIBLE,
                                    CONSTRAINT `prov_no_m_fk`
                                      FOREIGN KEY (`prov_no`)
                                      REFERENCES `provinces` (`prov_no`)
                                      ON DELETE NO ACTION
                                      ON UPDATE NO ACTION )
                                    COMMENT='Philippines municipalities'
                                    ENGINE=InnoDB";

                    try
                    {
                        cmd.ExecuteNonQuery();
                        tableCount++;
                    }
                    catch (MySqlException msex)
                    {

                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex);
                    }
                    #endregion

                    #region Landing sites
                    cmd.CommandText = @" CREATE TABLE IF NOT EXISTS `landing_sites`(
                                        `landing_site_id` INT NOT NULL,
                                        `landing_site_name` VARCHAR(100) NOT NULL,
                                        `municipality` INT NOT NULL,
                                        `latitude` DOUBLE NULL,
                                        `longitude` DOUBLE NULL,
                                        `barangay` VARCHAR(40) NULL,
                                        PRIMARY KEY (`landing_site_id`),
                                        INDEX `muni_ls_fk_idx` (`municipality` ASC) VISIBLE,
                                        CONSTRAINT `muni_ls_fk`
                                          FOREIGN KEY (`municipality`)
                                          REFERENCES `municipalities` (`mun_no`)
                                          ON DELETE NO ACTION
                                          ON UPDATE NO ACTION )
                                        COMMENT='Landing sites monitored for fish landings'
                                        ENGINE=InnoDB";

                    try
                    {
                        cmd.ExecuteNonQuery();
                        tableCount++;
                    }
                    catch (MySqlException msex)
                    {

                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex);
                    }
                    #endregion

                    #region Effort specification
                    cmd.CommandText = @"CREATE TABLE IF NOT EXISTS `effort_specification`(
                                        `effort_specification_id` INT NOT NULL,
                                        `effort_specification` VARCHAR(40) NOT NULL,
                                        `for_all_types_of_fishing` TINYINT(1) NOT NULL,
                                        `value_type` TINYINT NOT NULL,
                                        PRIMARY KEY (`effort_specification_id`),
                                        UNIQUE INDEX `effort_spec_idx` (`effort_specification` ASC) VISIBLE )
                                        COMMENT='Fishing effort specifiers'
                                        ENGINE=InnoDB";

                    try
                    {
                        cmd.ExecuteNonQuery();
                        tableCount++;
                    }
                    catch (MySqlException msex)
                    {

                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex);
                    }
                    #endregion

                    #region Gear
                    cmd.CommandText = @"CREATE TABLE IF NOT EXISTS `gears`(
                                        `gear_code` VARCHAR(6) NOT NULL,
                                        `gear_name` VARCHAR(40) NOT NULL,
                                        `generic_code` VARCHAR(6) NOT NULL,
                                        `is_generic` TINYINT(1) NOT NULL,
                                        PRIMARY KEY (`gear_code`),
                                        UNIQUE INDEX `gear_name_idx` (`gear_name` ASC) VISIBLE )
                                        COMMENT='Fishing gears'
                                        ENGINE=InnoDB";

                    try
                    {
                        cmd.ExecuteNonQuery();
                        tableCount++;
                    }
                    catch (MySqlException msex)
                    {

                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex);
                    }
                    #endregion

                    #region gear effort specification
                    cmd.CommandText = @"CREATE TABLE IF NOT EXISTS `gear_effort_specification`(
                                        `gear` VARCHAR(6) NOT NULL,
                                        `effort_spec` INT NOT NULL,
                                        `row_id` INT NOT NULL,
                                        PRIMARY KEY (`row_id`),
                                        INDEX `gear_ges_fk_idx` (`gear` ASC) VISIBLE,
                                        INDEX `effort_spec_ges_fk_idx` (`effort_spec` ASC) VISIBLE,
                                        UNIQUE KEY `altKey_gef` (`gear`,`effort_spec`),
                                        CONSTRAINT `gear_ges_fk`
                                          FOREIGN KEY (`gear`)
                                          REFERENCES `gears` (`gear_code`)
                                          ON DELETE NO ACTION
                                          ON UPDATE NO ACTION,
                                        CONSTRAINT `effort_spec_ges_fk`
                                          FOREIGN KEY (`effort_spec`)
                                          REFERENCES `effort_specification` (`effort_specification_id`)
                                          ON DELETE NO ACTION
                                          ON UPDATE NO ACTION )
                                          COMMENT='Effort specifications of fishing gears'
                                        ENGINE=InnoDB";

                    try
                    {
                        cmd.ExecuteNonQuery();
                        tableCount++;
                    }
                    catch (MySqlException msex)
                    {

                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex);
                    }
                    #endregion

                    #region Fishing grounds
                    cmd.CommandText = @" CREATE TABLE IF NOT EXISTS `fishing_grounds`(
                                        `fishing_ground_code` VARCHAR(6) NOT NULL,
                                        `fishing_ground_name` VARCHAR (40) NOT NULL,
                                        PRIMARY KEY (`fishing_ground_code`),
                                        UNIQUE INDEX `fishing_ground_name_idx` (`fishing_ground_name` ASC) VISIBLE )
                                        COMMENT='Fishing grounds'
                                        ENGINE=InnoDB";

                    try
                    {
                        cmd.ExecuteNonQuery();
                        tableCount++;
                    }
                    catch (MySqlException msex)
                    {

                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex);
                    }
                    #endregion

                    #region NSAPREgion FMA Fishing grounds
                    cmd.CommandText = @"CREATE TABLE IF NOT EXISTS `nsap_region_fma_fishing_grounds` (
                                        `region_fma` INT NOT NULL,
                                        `fishing_ground` VARCHAR(6) NOT NULL,
                                        `row_id` INT NOT NULL,
                                        `date_start` DATE NOT NULL,
                                        `date_end` DATE NULL,
                                        PRIMARY KEY (`row_id`),
                                        INDEX `fishing_ground_nffg_fk_idx` (`fishing_ground` ASC) VISIBLE,
                                        INDEX `region_nffg_fk_idx` (`region_fma` ASC) VISIBLE,
                                        UNIQUE KEY `altKey_nrfg` (`region_fma`,`fishing_ground`),
                                        CONSTRAINT `fishing_ground_nffg_fk`
                                          FOREIGN KEY (`fishing_ground`)
                                          REFERENCES `fishing_grounds` (`fishing_ground_code`)
                                          ON DELETE NO ACTION
                                          ON UPDATE NO ACTION,
                                        CONSTRAINT `region_nffg_fk`
                                          FOREIGN KEY (`region_fma`)
                                          REFERENCES `nsap_region_fma` (`row_id`) 
                                          ON DELETE NO ACTION
                                          ON UPDATE NO ACTION )
                                        COMMENT='Fishing grounds in FMAs in NSAP regions'
                                        ENGINE=InnoDB";

                    try
                    {
                        cmd.ExecuteNonQuery();
                        tableCount++;
                    }
                    catch (MySqlException msex)
                    {

                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex);
                    }
                    #endregion

                    #region NSAP Region landing site
                    cmd.CommandText = @"CREATE TABLE IF NOT EXISTS `nsap_region_landing_site`(
                                        `row_id` INT NOT NULL,
                                        `nsap_region_fma_fg` INT NOT NULL,
                                        `landing_site` INT NOT NULL,
                                        `date_start` DATE NOT NULL,
                                        `date_end` DATE NULL,
                                        PRIMARY KEY (`row_id`),
                                        INDEX `nsap_region_fma_fg_ls_fk_idx` (`nsap_region_fma_fg` ASC) VISIBLE,
                                        INDEX `landing_site_fg_fk_idx` (`landing_site` ASC) VISIBLE,
                                        UNIQUE KEY `altKey_nrls` (`nsap_region_fma_fg`,`landing_site`),
                                        CONSTRAINT `nsap_region_fma_fg_ls_fk`
                                          FOREIGN KEY (`nsap_region_fma_fg`)
                                          REFERENCES `nsap_region_fma_fishing_grounds` (`row_id`)
                                          ON DELETE NO ACTION
                                          ON UPDATE NO ACTION,
                                        CONSTRAINT `landing_site_fg_fk`
                                          FOREIGN KEY (`landing_site`)
                                          REFERENCES `landing_sites` (`landing_site_id`)
                                          ON DELETE NO ACTION
                                          ON UPDATE NO ACTION )
                                        COMMENT='Landing site in fishing grounds in FMAs in NSAP regions'
                                        ENGINE=InnoDB";

                    try
                    {
                        cmd.ExecuteNonQuery();
                        tableCount++;
                    }
                    catch (MySqlException msex)
                    {

                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex);
                    }
                    #endregion

                    #region NSAP Region Gear
                    cmd.CommandText = @"CREATE TABLE IF NOT EXISTS `nsap_region_gear` (
                                        `nsap_region` VARCHAR(6) NOT NULL,
                                        `gear` VARCHAR(6) NOT NULL,
                                        `row_id` INT NOT NULL,
                                        `date_start` DATE NOT NULL,
                                        `date_end`DATE NULL,
                                        PRIMARY KEY (`row_id`),
                                        INDEX `nsap_region_nrg_fk_idx` (`nsap_region` ASC) VISIBLE,
                                        INDEX `gear_nrg_fk_idx` (`gear` ASC) VISIBLE,
                                        UNIQUE KEY `altKey_nrg`(`nsap_region`,`gear`),
                                        CONSTRAINT `nsap_region_nrg_fk`
                                          FOREIGN KEY (`nsap_region`)
                                          REFERENCES `nsap_region` (`code`)
                                          ON DELETE NO ACTION
                                          ON UPDATE NO ACTION,
                                        CONSTRAINT `gear_nrg_fk`
                                          FOREIGN KEY (`gear`)
                                          REFERENCES `gears` (`gear_code`)
                                          ON DELETE NO ACTION
                                          ON UPDATE NO ACTION )
                                        COMMENT='Gears in NSAP region'
                                        ENGINE=InnoDB";

                    try
                    {
                        cmd.ExecuteNonQuery();
                        tableCount++;
                    }
                    catch (MySqlException msex)
                    {

                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex);
                    }
                    #endregion

                    #region GPS
                    cmd.CommandText = @"CREATE TABLE IF NOT EXISTS `GPS`(
                                        `gps_code` VARCHAR(6) NOT NULL,
                                        `assigned_name` VARCHAR(30) NOT NULL,
                                        `brand` VARCHAR(30) NOT NULL,
                                        `model` VARCHAR(30) NOT NULL,
                                        `device_type` TINYINT NOT NULL,
                                        PRIMARY KEY (`gps_code`),
                                        UNIQUE INDEX `assigned_name_idx` (`assigned_name` ASC) VISIBLE )
                                        COMMENT='GPS and other tracking devices'
                                        ENGINE=InnoDB";

                    try
                    {
                        cmd.ExecuteNonQuery();
                        tableCount++;
                    }
                    catch (MySqlException msex)
                    {

                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex);
                    }
                    #endregion

                    #region Engine
                    cmd.CommandText = @" CREATE TABLE IF NOT EXISTS `engines`(
                                        `engine_id` INT NOT NULL,
                                        `manufacturer_name` VARCHAR(30) NULL,
                                        `model_name` VARCHAR(30) NULL,
                                        `horsepower` DOUBLE NOT NULL,
                                        PRIMARY KEY (`engine_id`) )
                                        COMMENT='Fishing vessel engine brands and horsepower'
                                        ENGINE=InnoDB";

                    try
                    {
                        cmd.ExecuteNonQuery();
                        tableCount++;
                    }
                    catch (MySqlException msex)
                    {

                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex);
                    }
                    #endregion

                    #region Fishing vessels
                    cmd.CommandText = @"CREATE TABLE IF NOT EXISTS `fishing_vessel` (
                                        `vessel_id` INT NOT NULL,
                                        `name_of_owner` VARCHAR(40) NULL,
                                        `vessel_name` VARCHAR(40) NULL,
                                        `length` DOUBLE NULL,
                                        `depth` DOUBLE NULL,
                                        `breadth` DOUBLE NULL,
                                        `registration_number` VARCHAR(40) NULL,
                                        `sector` VARCHAR(3) NULL,
                                        PRIMARY KEY (`vessel_id`) )
                                        COMMENT='Fishing vessels'
                                        ENGINE=InnoDB";

                    try
                    {
                        cmd.ExecuteNonQuery();
                        tableCount++;
                    }
                    catch (MySqlException msex)
                    {

                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex);
                    }
                    #endregion

                    #region NSAP region vessel
                    cmd.CommandText = @"CREATE TABLE IF NOT EXISTS `nsap_region_vessel`(
                                        `vessel` INT NOT NULL,
                                        `region` VARCHAR(6) NOT NULL,
                                        `row_id` INT NOT NULL,
                                        `date_start` DATE NOT NULL,
                                        `date_end` DATE NULL,
                                        PRIMARY KEY (`row_id`),
                                        INDEX `vessel_nrv_fk_idx` (`vessel` ASC) VISIBLE,
                                        INDEX `region_nrv_fk_idx` (`region` ASC) VISIBLE,
                                        UNIQUE KEY `altKey_nrv`(`vessel`,`region`),
                                        CONSTRAINT `vessel_nrv_fk`
                                          FOREIGN KEY (`vessel`)
                                          REFERENCES `fishing_vessel` (`vessel_id`)
                                          ON DELETE NO ACTION
                                          ON UPDATE NO ACTION,
                                        CONSTRAINT `region_nrv_fk`
                                          FOREIGN KEY (`region`)
                                          REFERENCES `nsap_region` (`code`)
                                          ON DELETE NO ACTION
                                          ON UPDATE NO ACTION )
                                        COMMENT='Fishing vessels in NSAP region'
                                        ENGINE=InnoDB";

                    try
                    {
                        cmd.ExecuteNonQuery();
                        tableCount++;
                    }
                    catch (MySqlException msex)
                    {

                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex);
                    }
                    #endregion

                    #region Fishbase species
                    cmd.CommandText = @"CREATE TABLE IF NOT EXISTS `fb_species` (
                                        `genus` VARCHAR(40) NOT NULL,
                                        `species` VARCHAR(40) NOT NULL,
                                        `spec_code` INT NOT NULL,
                                        PRIMARY KEY (`genus`, `species`),
                                        UNIQUE INDEX `spec_code_idx`(`spec_code` ASC) VISIBLE )
                                        COMMENT='Species list in FishBase'
                                        ENGINE=InnoDB";

                    try
                    {
                        cmd.ExecuteNonQuery();
                        tableCount++;
                    }
                    catch (MySqlException msex)
                    {

                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex);
                    }
                    #endregion

                    #region PH fish species
                    cmd.CommandText = @"CREATE TABLE IF NOT EXISTS `ph_fish` (
                                        `row_no` INT NOT NULL,
                                        `genus` VARCHAR(40) NOT NULL,
                                        `species` VARCHAR(40) NOT NULL,
                                        `species_id` INT NULL,
                                        `importance` VARCHAR(80) NOT NULL,
                                        `family` VARCHAR(40) NOT NULL,
                                        `main_catching_method` VARCHAR(80) NOT NULL,
                                        `length_common` DOUBLE NULL,
                                        `length_max` DOUBLE NULL,
                                        `length_max_length_type` VARCHAR(3) NULL,
                                        `length_type` VARCHAR(3) NULL,
                                        PRIMARY KEY (`row_no`),
                                        UNIQUE INDEX `genus_species_idx` (`genus`, `species` ASC) VISIBLE,
                                        INDEX `species_id_idx` (`species_id` ASC) VISIBLE )
                                        COMMENT='Fish species found in Philippines and confirmed by World Fish'
                                        ENGINE=InnoDB";

                    try
                    {
                        cmd.ExecuteNonQuery();
                        tableCount++;
                    }
                    catch (MySqlException msex)
                    {

                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex);
                    }
                    #endregion

                    #region Taxa
                    cmd.CommandText = @"CREATE TABLE IF NOT EXISTS `taxa`(
                                        `taxa_code` VARCHAR(6) NOT NULL,
                                        `taxa` VARCHAR(40) NOT NULL,
                                        `description` VARCHAR(80) NULL,
                                        PRIMARY KEY (`taxa_code`),
                                        UNIQUE INDEX `taxa_unique_idx` (`taxa` ASC) VISIBLE )
                                        COMMENT='Taxonomic categories of catch'
                                        ENGINE=InnoDB";

                    try
                    {
                        cmd.ExecuteNonQuery();
                        tableCount++;
                    }
                    catch (MySqlException msex)
                    {

                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex);
                    }

                    #endregion

                    #region Size types
                    cmd.CommandText = @"CREATE TABLE IF NOT EXISTS `size_types` (
                                        `size_type_code` VARCHAR(3) NOT NULL,
                                        `size_type_name` VARCHAR(40) NOT NULL,
                                        PRIMARY KEY (`size_type_code`),
                                        UNIQUE INDEX `size_type_name_unique_idx` (`size_type_name` ASC) VISIBLE )
                                        COMMENT='Size categories of catch'
                                        ENGINE=InnoDB";

                    try
                    {
                        cmd.ExecuteNonQuery();
                        tableCount++;
                    }
                    catch (MySqlException msex)
                    {

                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex);
                    }
                    #endregion

                    #region Not fish species
                    cmd.CommandText = @" CREATE TABLE IF NOT EXISTS `not_fish_species`(
                                        `species_id` INT NOT NULL,
                                        `genus` VARCHAR(40) NOT NULL,
                                        `species` VARCHAR(40) NOT NULL,
                                        `taxa` VARCHAR(6) NOT NULL,
                                        `size_indicator` VARCHAR(3) NULL,
                                        `max_size` DOUBLE NULL,
                                        PRIMARY KEY (`species_id`),
                                        UNIQUE INDEX `genus_species_nf_idx` (`genus`,`species` ASC) VISIBLE,
                                        INDEX `taxa_nf_fk_idx` (`taxa` ASC) VISIBLE,
                                        INDEX `size_indicator_nf_fk_idx` (`size_indicator`),
                                        CONSTRAINT `taxa_nf_fk`
                                          FOREIGN KEY (`taxa`)
                                          REFERENCES `taxa` (`taxa_code`)
                                          ON DELETE NO ACTION
                                          ON UPDATE NO ACTION,
                                        CONSTRAINT `size_indicator_fk`
                                          FOREIGN KEY (`size_indicator`)
                                          REFERENCES `size_types` (`size_type_code`)
                                          ON DELETE NO ACTION
                                          ON UPDATE NO ACTION )
                                       COMMENT='Non-fish catch mostly crustaceans'
                                        ENGINE=InnoDB";

                    try
                    {
                        cmd.ExecuteNonQuery();
                        tableCount++;
                    }
                    catch (MySqlException msex)
                    {

                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex);
                    }
                    #endregion

                    #region landing site sampling
                    cmd.CommandText = @"CREATE TABLE IF NOT EXISTS `dbo_lc_fg_sample_day`(
                                    `unload_day_id` INT NOT NULL,
                                    `region_id` VARCHAR(6) NOT NULL,
                                    `fma` INT NOT NULL,
                                    `sdate` DATE NOT NULL,
                                    `land_ctr_id`INT NULL,
                                    `ground_id` VARCHAR(6) NOT NULL,
                                    `remarks` VARCHAR(80) NULL,
                                    `is_sample_day` TINYINT(1) NOT NULL,
                                    `land_ctr_text` VARCHAR(150) NULL,
                                    PRIMARY KEY (`unload_day_id`),
                                    UNIQUE KEY `altKey` (`land_ctr_id`,`ground_id`,`sdate`),
                                    INDEX `region_id_lss_fk_idx` (`region_id` ASC) VISIBLE,
                                    INDEX `fma_lss_fk_idx` (`fma` ASC) VISIBLE,
                                    INDEX `land_ctr_id_lss_fk_idx` (`land_ctr_id` ASC) VISIBLE,
                                    INDEX `ground_id_lss_fk_idx` (`land_ctr_id` ASC) VISIBLE,
                                    CONSTRAINT `region_id_lss_fk`
                                      FOREIGN KEY (`region_id`)
                                      REFERENCES `nsap_region` (`code`)
                                      ON DELETE NO ACTION
                                      ON UPDATE NO ACTION,
                                    CONSTRAINT `fma_lss_fk`
                                      FOREIGN KEY (`fma`)
                                      REFERENCES `fma` (`fma_id`)
                                      ON DELETE NO ACTION
                                      ON UPDATE NO ACTION,
                                    CONSTRAINT `land_ctr_id_lss_fk`
                                      FOREIGN KEY (`land_ctr_id`)
                                      REFERENCES `landing_sites` (`landing_site_id`)
                                      ON DELETE NO ACTION
                                      ON UPDATE NO ACTION,
                                    CONSTRAINT `ground_id_lss_fk`
                                      FOREIGN KEY (`ground_id`)
                                      REFERENCES `fishing_grounds` (`fishing_ground_code`)
                                      ON DELETE NO ACTION
                                      ON UPDATE NO ACTION )
                                    COMMENT='Information related to a day of sampling'
                                    ENGINE=InnoDB";

                    try
                    {
                        cmd.ExecuteNonQuery();
                        tableCount++;
                    }
                    catch (MySqlException msex)
                    {

                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex);
                    }
                    #endregion

                    #region landing site sampling 1
                    cmd.CommandText = @"CREATE TABLE IF NOT EXISTS `dbo_lc_fg_sample_day_1`(
                                        `unload_day_id` INT NOT NULL,
                                        `datetime_submitted`DATETIME NULL,
                                        `user_name` VARCHAR(30) NULL,
                                        `device_id` VARCHAR(30) NULL,
                                        `xform_identifier` VARCHAR(30) NULL,
                                        `date_added` DATETIME NULL,
                                        `from_excel_download` TINYINT(1) NULL,
                                        `form_version` VARCHAR(30) NULL,
                                        `row_id` VARCHAR(30) NULL,
                                        `enumerator_id` INT NULL,
                                        `enumerator_text` VARCHAR(30) NULL,
                                        PRIMARY KEY (`unload_day_id`),
                                        INDEX `enumerator_id_lss_fk_idx` (`enumerator_id` ASC) VISIBLE,
                                        UNIQUE INDEX `row_id_UNIQUE` (`row_id` ASC) VISIBLE,
                                        CONSTRAINT `enumerator_id_lss_fk`
                                          FOREIGN KEY (`enumerator_id`)
                                          REFERENCES `nsap_enumerators` (`enumerator_id`)
                                          ON DELETE NO ACTION
                                          ON UPDATE NO ACTION,
                                        CONSTRAINT `unload_day_id_lss_fk`
                                          FOREIGN KEY (`unload_day_id`)
                                          REFERENCES `dbo_LC_FG_sample_day` (`unload_day_id`)
                                          ON DELETE NO ACTION
                                          ON UPDATE NO ACTION )
                                        COMMENT='Device metadata collected when capturing number of landed boats and total catch per landing site'
                                        ENGINE=InnoDB";

                    try
                    {
                        cmd.ExecuteNonQuery();
                        tableCount++;
                    }
                    catch (MySqlException msex)
                    {

                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex);
                    }
                    #endregion  

                    #region Gear unload
                    cmd.CommandText = @"CREATE TABLE IF NOT EXISTS `dbo_gear_unload` (
                                        `unload_gr_id` INT NOT NULL,
                                        `unload_day_id` INT NOT NULL,
                                        `gr_id` VARCHAR(6) NULL,
                                        `boats` INT NULL,
                                        `catch` DOUBLE NULL,
                                        `gr_text` VARCHAR(60) NULL,
                                        `remarks` VARCHAR(80) NULL,
                                        PRIMARY KEY (`unload_gr_id`),
                                        INDEX `unload_day_id_gu_fk_idx` (`unload_day_id` ASC) VISIBLE,
                                        INDEX `gr_id_gu_fk_idx` (`gr_id` ASC) VISIBLE,
                                        UNIQUE KEY `altKey_gu` (`gr_id`,`unload_day_id`),
                                        CONSTRAINT `unload_day_id_gu_fk`
                                          FOREIGN KEY (`unload_day_id`)
                                          REFERENCES `dbo_LC_FG_sample_day` (`unload_day_id`)
                                          ON DELETE NO ACTION
                                          ON UPDATE NO ACTION,
                                        CONSTRAINT `gr_id_gu_fk`
                                          FOREIGN KEY (`gr_id`)
                                          REFERENCES `gears` (`gear_code`)
                                          ON DELETE NO ACTION
                                          ON UPDATE NO ACTION )
                                        COMMENT='Gear sampled with number of boats and total catch using the gear'
                                        ENGINE=InnoDB";

                    try
                    {
                        cmd.ExecuteNonQuery();
                        tableCount++;
                    }
                    catch (MySqlException msex)
                    {

                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex);
                    }
                    #endregion

                    #region Vessel unload
                    cmd.CommandText = @"CREATE TABLE IF NOT EXISTS `dbo_vessel_unload`(
                                    `unload_gr_id` INT NOT NULL,
                                    `v_unload_id` INT NOT NULL,
                                    `boat_id` INT NULL,
                                    `catch_total` DOUBLE NULL,
                                    `catch_samp` DOUBLE NULL,
                                    `boxes_total` INT NULL,
                                    `boxes_samp` INT NULL,
                                    `boat_text` VARCHAR(40) NULL,
                                    `is_boat_used` TINYINT(1),
                                    `raising_factor` DOUBLE NULL,
                                    PRIMARY KEY (`v_unload_id`),
                                    INDEX `unload_gr_id_vu_fk_idx` (`unload_gr_id` ASC) VISIBLE,
                                    INDEX `boat_id_vu_fk_idx` (`boat_id` ASC) VISIBLE,
                                    CONSTRAINT `unload_gr_id_vu_fk`
                                      FOREIGN KEy (`unload_gr_id`)
                                      REFERENCES dbo_gear_unload (`unload_gr_id`)
                                      ON DELETE NO ACTION
                                      ON UPDATE NO ACTION,
                                    CONSTRAINT `boat_id_vu_fk`
                                      FOREIGN KEy (`boat_id`)
                                      REFERENCES fishing_vessel (`vessel_id`)
                                      ON DELETE NO ACTION
                                      ON UPDATE NO ACTION )
                                    COMMENT='Data captured for each sampled landing'
                                    ENGINE=InnoDB";
                    try
                    {
                        cmd.ExecuteNonQuery();
                        tableCount++;
                    }
                    catch (MySqlException msex)
                    {

                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex);
                    }
                    #endregion

                    #region Vessel unload 1
                    cmd.CommandText = @"CREATE TABLE IF NOT EXISTS `dbo_vessel_unload_1` (
                                        `v_unload_id` INT NOT NULL,
                                        `success` TINYINT(1),
                                        `tracked` TINYINT(1),
                                        `trip_is_completed` TINYINT(1),
                                        `departure_landing_site` DATETIME NULL,
                                        `arrival_landing_site` DATETIME NULL,
                                        `sector_code` VARCHAR(2) NULL,
                                        `row_id` VARCHAR(38) NULL,
                                        `xform_identifier` VARCHAR(38) NULL,
                                        `xform_date` DATETIME NULL,
                                        `user_name` VARCHAR(30) NULL,
                                        `device_id` VARCHAR(30) NULL,
                                        `datetime_submitted` DATETIME NULL,
                                        `form_version` VARCHAR(30) NULL,
                                        `gps` VARCHAR(20),
                                        `sampling_date` DATETIME NULL,
                                        `notes` VARCHAR(2000) NULL,
                                        `enumerator_id` INT NULL,
                                        `enumerator_text` VARCHAR(40) NULL,
                                        `date_added` DATETIME NULL,
                                        `from_excel_download` TINYINT(1),
                                        `has_catch_composition` TINYINT(1),
                                        `number_of_fishers` INT NULL,
                                        PRIMARY KEY (`v_unload_id`),
                                        UNIQUE INDEX `row_id_UNIQUE` (`row_id` ASC) VISIBLE,
                                        INDEX `gps_vu1_fk_idx` (`gps` ASC) VISIBLE,
                                        INDEX `enumerator_id_vu1_fk_idx` (`enumerator_id` ASC) VISIBLE,
                                        CONSTRAINT `gps_vu1_fk` 
                                          FOREIGN KEY (`gps`)
                                          REFERENCES `gps` (`gps_code`)
                                          ON DELETE NO ACTION
                                          ON UPDATE NO ACTION,
                                        CONSTRAINT `enumerator_id_vu1_fk` 
                                          FOREIGN KEY (`enumerator_id`)
                                          REFERENCES `nsap_enumerators` (`enumerator_id`)
                                          ON DELETE NO ACTION
                                          ON UPDATE NO ACTION,
                                        CONSTRAINT `v_unload_id_vu1_fk` 
                                          FOREIGN KEY (`v_unload_id`)
                                          REFERENCES `dbo_vessel_unload` (`v_unload_id`)
                                          ON DELETE NO ACTION
                                          ON UPDATE NO ACTION )
                                        COMMENT='Other data including device metadata captured when sampling a landing'
                                        ENGINE=InnoDB";

                    try
                    {
                        cmd.ExecuteNonQuery();
                        tableCount++;
                    }
                    catch (MySqlException msex)
                    {

                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex);
                    }
                    #endregion

                    #region fishing ground grid
                    cmd.CommandText = @" CREATE TABLE IF NOT EXISTS `dbo_fg_grid`(
                                        `fg_grid_id` INT NOT NULL,
                                        `v_unload_id` INT NOT NULL,
                                        `utm_zone` VARCHAR(3) NOT NULL,
                                        `grid25`VARCHAR(10) NOT NULL,
                                        PRIMARY KEY (`fg_grid_id`),
                                        INDEX `v_unload_id_fgg_fk_idx` (`v_unload_id` ASC) VISIBLE,
                                        UNIQUE KEY `altKey_fgg` (`grid25`,`v_unload_id`),
                                        CONSTRAINT `v_unload_id_fgg_fk`
                                          FOREIGN KEY (`v_unload_id`)
                                          REFERENCES `dbo_vessel_unload` (`v_unload_id`)
                                          ON DELETE NO ACTION
                                          ON UPDATE NO ACTION )
                                        COMMENT='Location of fishing ground based on a 2x2 kilometer grid'
                                        ENGINE=InnoDB";

                    try
                    {
                        cmd.ExecuteNonQuery();
                        tableCount++;
                    }
                    catch (MySqlException msex)
                    {

                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex);
                    }
                    #endregion

                    #region gear soak
                    cmd.CommandText = @"CREATE TABLE IF NOT EXISTS `dbo_gear_soak` (
                                        `gear_soak_id` INT NOT NULL,
                                        `v_unload_id` INT NOT NULL,
                                        `time_set` DATETIME NULL,
                                        `time_hauled`DATETIME NULL,
                                        `wpt_set` VARCHAR(12) NULL,
                                        `wpt_haul` VARCHAR(12) NULL,
                                        PRIMARY KEY (`gear_soak_id`),
                                        INDEX `v_unload_id_gs_fk_idx` (`v_unload_id` ASC) VISIBLE,
                                        CONSTRAINT `v_unload_id_gs_fk`
                                          FOREIGN KEY (`v_unload_id`)
                                          REFERENCES `dbo_vessel_unload` (`v_unload_id`)
                                          ON DELETE NO ACTION
                                          ON UPDATE NO ACTION )
                                        COMMENT='Soak time of fishing gear for each fishing operation'
                                        ENGINE=InnoDB";
                    try
                    {
                        cmd.ExecuteNonQuery();
                        tableCount++;
                    }
                    catch (MySqlException msex)
                    {

                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex);
                    }
                    #endregion

                    #region vessel effort
                    cmd.CommandText = @"CREATE TABLE IF NOT EXISTS `dbo_vessel_effort`(
                                        `effort_row_id` INT NOT NULL,
                                        `v_unload_id` INT NOT NULL,
                                        `effort_spec_id` INT NOT NULL,
                                        `effort_value_numeric` DOUBLE NULL,
                                        `effort_value_text` VARCHAR(30) NULL,
                                        PRIMARY KEY (`effort_row_id`),
                                        INDEX `v_unload_id_ve_fk_idx` (`v_unload_id` ASC) VISIBLE,
                                        CONSTRAINT `v_unload_id_ve_fk`
                                          FOREIGN KEY (`v_unload_id`)
                                          REFERENCES `dbo_vessel_unload` (`v_unload_id`)
                                          ON DELETE NO ACTION
                                          ON UPDATE NO ACTION )
                                        COMMENT='Values of fishing effort for each operation'
                                        ENGINE=InnoDB";

                    try
                    {
                        cmd.ExecuteNonQuery();
                        tableCount++;
                    }
                    catch (MySqlException msex)
                    {

                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex);
                    }
                    #endregion

                    #region vessel catch
                    cmd.CommandText = @"CREATE TABLE IF NOT EXISTS `dbo_vessel_catch`(
                                        `catch_id` INT NOT NULL,
                                        `v_unload_id` INT NOT NULL,
                                        `species_id` INT NULL,
                                        `catch_kg` DOUBLE NULL,
                                        `tws` DOUBLE NULL,
                                        `samp_kg` DOUBLE NULL,
                                        `taxa` VARCHAR(6) NULL,
                                        `species_text` VARCHAR(200) NULL,
                                        PRIMARY KEY (`catch_id`),
                                        INDEX `v_unload_id_vc_fk_idx` (`v_unload_id` ASC) VISIBLE,
                                        INDEX `species_id_vc_idx` (`species_id` ASC) VISIBLE,
                                        INDEX `taxa_vc__fk_idx` (`taxa` ASC) VISIBLE,
                                        CONSTRAINT `v_unload_id_vc_fk`
                                          FOREIGN KEY (`v_unload_id`)
                                          REFERENCES `dbo_vessel_unload` (`v_unload_id`)
                                          ON DELETE NO ACTION
                                          ON UPDATE NO ACTION,
                                        CONSTRAINT `taxa_vc_fk`
                                          FOREIGN KEY (`taxa`)
                                          REFERENCES `taxa` (`taxa_code`)
                                          ON DELETE NO ACTION
                                          ON UPDATE NO ACTION )
                                        COMMENT='Catch composition of sampled landing'
                                        ENGINE=InnoDB";

                    try
                    {
                        cmd.ExecuteNonQuery();
                        tableCount++;
                    }
                    catch (MySqlException msex)
                    {

                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex);
                    }
                    #endregion

                    #region catch length
                    cmd.CommandText = @" CREATE TABLE IF NOT EXISTS `dbo_catch_length`(
                                    `catch_len_id` INT NOT NULL,
                                    `catch_id` INT NOT NULL,
                                    `length` DOUBLE NOT NULL,
                                    PRIMARY KEY (`catch_len_id`),
                                    INDEX `catch_id_cl_fk_idx` (`catch_id` ASC) VISIBLE,
                                    CONSTRAINT `catch_id_cl_fk`
                                      FOREIGN KEY (`catch_id`)
                                      REFERENCES `dbo_vessel_catch` (`catch_id`)
                                      ON DELETE NO ACTION
                                      ON UPDATE NO ACTION )
                                    COMMENT='Lengths of catch'
                                    ENGINE=InnoDB";

                    try
                    {
                        cmd.ExecuteNonQuery();
                        tableCount++;
                    }
                    catch (MySqlException msex)
                    {

                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex);
                    }
                    #endregion

                    #region catch maturity
                    cmd.CommandText = @"CREATE TABLE IF NOT EXISTs `dbo_catch_maturity`(
                                    `catch_maturity_id` INT NOT NULL,
                                    `catch_id` INT NOT NULL,
                                    `length` DOUBLE NULL,
                                    `weight` DOUBLE NULL,
                                    `sex` VARCHAR(1) NULL,
                                    `maturity` VARCHAR(3) NULL,
                                    `gut_content_wt` DOUBLE NULL,
                                    `gut_content_class` VARCHAR(2) NULL,
                                    `gonad_wt` DOUBLE NULL,
                                    PRIMARY KEY (`catch_maturity_id`),
                                    INDEX `catch_id_cm_fk_idx` (`catch_id` ASC) VISIBLE,
                                    CONSTRAINT `catch_id_cm_fk`
                                      FOREIGN KEY (`catch_id`)
                                      REFERENCES `dbo_vessel_catch` (`catch_id`)
                                      ON DELETE NO ACTION
                                      ON UPDATE NO ACTION )
                                    COMMENT='Maturity indicators of catch'
                                    ENGINE=InnoDB";

                    try
                    {
                        cmd.ExecuteNonQuery();
                        tableCount++;
                    }
                    catch (MySqlException msex)
                    {

                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex);
                    }
                    #endregion

                    #region catch len freq
                    cmd.CommandText = @" CREATE TABLE IF NOT EXISTS `dbo_catch_len_freq`(
                                        `catch_lf_id` INT NOT NULL,
                                        `catch_id` INT NOT NULL,
                                        `length` DOUBLE NOT NULL,
                                        `freq` INT NOT NULL,
                                        PRIMARY KEY (`catch_lf_id`),
                                        INDEX `catch_id_clf_fk_idx`(`catch_id` ASC) VISIBLE,
                                        CONSTRAINT `catch_id_clf_fk`
                                          FOREIGN KEY (`catch_id`)
                                          REFERENCES  `dbo_vessel_catch` (`catch_id`)
                                          ON DELETE NO ACTION
                                          ON UPDATE NO ACTION )
                                        COMMENT='Length frequency data of catch'
                                        ENGINE=InnoDB";

                    try
                    {
                        cmd.ExecuteNonQuery();
                        tableCount++;
                    }
                    catch (MySqlException msex)
                    {

                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex);
                    }
                    #endregion

                    #region catch length weight
                    cmd.CommandText = @" CREATE TABLE IF NOT EXISTS `dbo_catch_len_wt`(
                                        `catch_lw_id` INT NOT NULL,
                                        `catch_id` INT NOT NULL,
                                        `length` DOUBLE NOT NULL,
                                        `weight` DOUBLE NOT NULL,
                                        PRIMARY KEY (`catch_lw_id`),
                                        INDEX `catch_id_clw_fk_idx`(`catch_id` ASC) VISIBLE,
                                        CONSTRAINT `catch_id_clw_fk`
                                          FOREIGN KEY (`catch_id`)
                                          REFERENCES  `dbo_vessel_catch` (`catch_id`)
                                          ON DELETE NO ACTION
                                          ON UPDATE NO ACTION )
                                        COMMENT='Length-weight data of catch'
                                        ENGINE=InnoDB";

                    try
                    {
                        cmd.ExecuteNonQuery();
                        tableCount++;
                    }
                    catch (MySqlException msex)
                    {

                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex);
                    }
                    #endregion

                    #region JSON Files
                    cmd.CommandText = @" CREATE TABLE IF NOT EXISTS `json_file`(
                                        `row_id` INT NOT NULL,
                                        `filename` VARCHAR(150) NOT NULL,
                                        `count` INT NOT NULL,
                                        `earliest_date` DATETIME NOT NULL,
                                        `latest_date` DATETIME NOT NULL,
                                        `date_added` DATETIME NOT NULL,
                                        `md5`VARCHAR(40) NOT NULL,
                                        `form_id` VARCHAR(20) NOT NULL,
                                        `description` VARCHAR(100) NOT NULL,
                                        PRIMARY KEY (`row_id`) )
                                        ENGINE=InnoDB";

                    try
                    {
                        cmd.ExecuteNonQuery();
                        tableCount++;
                    }
                    catch (MySqlException msex)
                    {

                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex);
                    }
                    #endregion

                    #region LandingStat
                    cmd.CommandText = @"CREATE TABLE IF NOT EXISTS `dbo_vessel_unload_stats` (
                                        `v_unload_id` INT NOT NULL,
                                        `count_effort` INT  NULL,
                                        `count_grid` INT  NULL,
                                        `count_soak` INT  NULL,
                                        `count_catch_composition` INT  NULL,
                                        `count_lengths` INT  NULL,
                                        `count_lenfreq` INT  NULL,
                                        `count_lenwt` INT  NULL,
                                        `count_maturity` INT  NULL,
                                        PRIMARY KEY (`v_unload_id`),
                                        CONSTRAINT `v_unload_id_vu2_fk` 
                                          FOREIGN KEY (`v_unload_id`)
                                          REFERENCES `dbo_vessel_unload` (`v_unload_id`)
                                          ON DELETE NO ACTION
                                          ON UPDATE NO ACTION )
                                        COMMENT='Summary statistics of a sampled landing'
                                        ENGINE=InnoDB";
                    try
                    {
                        cmd.ExecuteNonQuery();
                        tableCount++;
                    }
                    catch (MySqlException msex)
                    {

                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex);
                    }
                    #endregion

                    #region koboservers
                    cmd.CommandText = @"CREATE TABLE IF NOT EXISTS  `kobo_servers` (
                                                `server_numeric_id` INT NOT NULL,
                                                `form_name` VARCHAR(150) NOT NULL,
                                                `server_id` VARCHAR(150) NOT NULL,
                                                `owner` VARCHAR(150) NOT NULL,
                                                `form_version` VARCHAR(150) NOT NULL,
                                                `e_form_version` VARCHAR(150) NULL,
                                                `date_created` DATETIME NOT NULL,
                                                `date_modified` DATETIME NOT NULL,
                                                `date_last_submission` DATETIME NOT NULL,
                                                `submission_count` INT NOT NULL,
                                                `user_count` INT NOT NULL,
                                                `date_last_accessed` DATETIME NOT NULL,
                                                `saved_in_db_count` INT NOT NULL,
                                                `last_uploaded_json` VARCHAR(150) NULL,
                                                `last_created_json` VARCHAR(150) NULL,
                                                PRIMARY KEY(`server_numeric_id`) )
                                                ENGINE=InnoDB";

                    try
                    {
                        cmd.ExecuteNonQuery();
                        tableCount++;
                    }
                    catch (MySqlException msex)
                    {

                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex);
                    }
                    #endregion
                }
            }

            return tableCount;
        }
    }
}
