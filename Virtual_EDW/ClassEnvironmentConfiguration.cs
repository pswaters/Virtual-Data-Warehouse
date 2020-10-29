﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Virtual_Data_Warehouse
{
    class EnvironmentConfiguration
    {
        /// <summary>
        ///    Check if the core VEDW paths exists and create them if necessary
        /// </summary>
        internal static void InitialiseVedwRootPath()
        {
            #region Create Root paths
            // Create the VEDW root configuration directory if it does not exist yet
            try
            {
                if (!Directory.Exists(FormBase.GlobalParameters.VedwConfigurationPath))
                {
                    Directory.CreateDirectory(FormBase.GlobalParameters.VedwConfigurationPath);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error creation default directory at " + FormBase.GlobalParameters.VedwConfigurationPath + " the message is " + ex, "An issue has been encountered", MessageBoxButtons.OK,MessageBoxIcon.Error);
            }

            // Create the output directory if it does not exist yet
            try
            {
                if (!Directory.Exists(FormBase.VedwConfigurationSettings.VedwOutputPath))
                {
                    Directory.CreateDirectory(FormBase.VedwConfigurationSettings.VedwOutputPath);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error creation default directory at " + FormBase.VedwConfigurationSettings.VedwOutputPath + " the message is " +ex, "An issue has been encountered", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            // Create the pattern directory if it does not exist yet
            try
            {
                if (!Directory.Exists(FormBase.VedwConfigurationSettings.VedwInputPath))
                {
                    Directory.CreateDirectory(FormBase.VedwConfigurationSettings.VedwInputPath);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error creation default directory at " + FormBase.VedwConfigurationSettings.VedwInputPath + " the message is " + ex, "An issue has been encountered", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
            #endregion

            #region Create Default Configuration File
            // If it doesn't exist already, create the core VEDW settings file with default values.
            try
            {
                if (!File.Exists(FormBase.GlobalParameters.VedwConfigurationPath + FormBase.GlobalParameters.VedwConfigurationfileName + FormBase.GlobalParameters.VedwFileExtension))
                {
                    var initialConfigurationFile = new StringBuilder();

                    initialConfigurationFile.AppendLine("/* Virtual Enterprise Data Warehouse (VEDW) Core Settings */");
                    initialConfigurationFile.AppendLine("TeamConfigurationPath|" + FormBase.VedwConfigurationSettings.TeamConfigurationPath); //Initially make this the same as the VEDW application root
                    initialConfigurationFile.AppendLine("VedwOutputPath|" + FormBase.VedwConfigurationSettings.VedwOutputPath);
                    initialConfigurationFile.AppendLine("InputPath|" + FormBase.VedwConfigurationSettings.VedwInputPath);
                    initialConfigurationFile.AppendLine("LoadPatternPath|" + FormBase.VedwConfigurationSettings.LoadPatternPath);
                    initialConfigurationFile.AppendLine("VedwSchema|vedw");
                    initialConfigurationFile.AppendLine("/* End of file */");

                    using (var outfile = new StreamWriter(FormBase.GlobalParameters.VedwConfigurationPath + FormBase.GlobalParameters.VedwConfigurationfileName + FormBase.GlobalParameters.VedwFileExtension))
                    {
                        outfile.Write(initialConfigurationFile.ToString());
                        outfile.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "An error occurred while creation the default path file. The error message is " + ex,
                    "An issue has been encountered", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            #endregion
        }


        /// <summary>
        /// Retrieve the values of the VEDW core settings file (where the paths to the TEAM configuration file are maintained)
        /// </summary>
        public static string LoadVedwSettingsFile(string filename)
        {
            string returnValue;
            string errorValue = "";

            // This is the hardcoded base path that always needs to be accessible, it has the main file which can locate the rest of the configuration
            var configList = new Dictionary<string, string>();
            var fs = new FileStream(filename, FileMode.Open, FileAccess.Read);
            var sr = new StreamReader(fs);

            try
            {
                string textline;
                while ((textline = sr.ReadLine()) != null)
                {
                    if (textline.IndexOf(@"/*", StringComparison.Ordinal) == -1 && textline.Trim() != "")
                    {
                        var line = textline.Split('|');
                        configList.Add(line[0], line[1]);
                    }
                }

                sr.Close();
                fs.Close();

                // Load the information from the VEDW settings file into memory
                int errorCounter = 0;
                string configurationValue;

                configurationValue = "TeamConfigurationPath";
                if (configList.ContainsKey(configurationValue))
                {
                    FormBase.VedwConfigurationSettings.TeamConfigurationPath = configList[configurationValue];
                }
                else
                {
                    errorValue = errorValue + $"* The entry {configurationValue} was not found in the configuration file. Please make sure an entry exists ({configurationValue}|<value>)\r\n.";
                    errorCounter++;
                }

                configurationValue = "LoadPatternPath";
                if (configList.ContainsKey(configurationValue))
                {
                    FormBase.VedwConfigurationSettings.LoadPatternPath = configList[configurationValue];
                }
                else
                {
                    errorValue = errorValue + $"* The entry {configurationValue} was not found in the configuration file. Please make sure an entry exists ({configurationValue}|<value>)\r\n.";
                    errorCounter++;
                }


                configurationValue = "VedwOutputPath";
                if (configList.ContainsKey(configurationValue))
                {
                    FormBase.VedwConfigurationSettings.VedwOutputPath = configList[configurationValue];
                }
                else
                {
                    errorValue = errorValue + $"* The entry {configurationValue} was not found in the configuration file. Please make sure an entry exists ({configurationValue}|<value>)\r\n.";
                    errorCounter++;
                }

                configurationValue = "InputPath";
                if (configList.ContainsKey(configurationValue))
                {
                    FormBase.VedwConfigurationSettings.VedwInputPath = configList[configurationValue];
                }
                else
                {
                    errorValue = errorValue + $"* The entry {configurationValue} was not found in the configuration file. Please make sure an entry exists ({configurationValue}|<value>).\r\n";
                    errorCounter++;
                }

                configurationValue = "VedwSchema";
                if (configList.ContainsKey(configurationValue))
                {
                    FormBase.VedwConfigurationSettings.VedwSchema = configList[configurationValue];
                }
                else
                {
                    errorValue = errorValue + $"* The entry {configurationValue} was not found in the configuration file. Please make sure an entry exists ({configurationValue}|<value>).\r\n";
                    errorCounter++;
                }

                returnValue = "The configuration file " + filename + " was loaded.";
                if (errorCounter>0)
                {
                    returnValue = returnValue+ $"\r\n\r\nHowever, the file was loaded with {errorCounter} error(s). The reported errors are:\r\n" + errorValue;
                }
            }
            catch (Exception ex)
            {
                returnValue = "There was an unhandled issue loading the configuration file. The file that was attempted to be loaded was: " + filename +"\r\n\r\n."+ "The exception message returned was "+ex;
            }
            return returnValue;
        }

        /// <summary>
        ///    Retrieve the TEAM configuration information from disk and save this to memory
        /// </summary>
        internal static StringBuilder LoadTeamConfigurationFile(string filename)
        {
            var returnCode = new StringBuilder(); // Collecting information to feedback to user.

            try
            {
                var configList = new Dictionary<string, string>();
                var fs = new FileStream(filename, FileMode.Open, FileAccess.Read);
                var sr = new StreamReader(fs);

                if (File.Exists(filename))
                {
                    string textline;
                    while ((textline = sr.ReadLine()) != null)
                    {
                        if (textline.IndexOf(@"/*", StringComparison.Ordinal) == -1 && textline.Trim() != "")
                        {
                            var line = textline.Split('|');
                            configList.Add(line[0], line[1]);
                        }
                    }

                    sr.Close();
                    fs.Close();

                    if (configList.Count == 0)
                    {
                        returnCode.AppendLine("No lines detected in file "+filename+". Is it empty?");
                    }

                    var connectionStringOmd = configList["connectionStringMetadata"];
                    connectionStringOmd = connectionStringOmd.Replace("Provider=SQLNCLI10;", "").Replace("Provider=SQLNCLI11;", "").Replace("Provider=SQLNCLI12;", "");

                    var connectionStringSource = configList["connectionStringSource"];
                    connectionStringSource = connectionStringSource.Replace("Provider=SQLNCLI10;", "")
                        .Replace("Provider=SQLNCLI11;", "").Replace("Provider=SQLNCLI12;", "");

                    var connectionStringStg = configList["connectionStringStaging"];
                    connectionStringStg = connectionStringStg.Replace("Provider=SQLNCLI10;", "")
                        .Replace("Provider=SQLNCLI11;", "").Replace("Provider=SQLNCLI12;", "");

                    var connectionStringHstg = configList["connectionStringPersistentStaging"];
                    connectionStringHstg = connectionStringHstg.Replace("Provider=SQLNCLI10;", "")
                        .Replace("Provider=SQLNCLI11;", "").Replace("Provider=SQLNCLI12;", "");

                    var connectionStringInt = configList["connectionStringIntegration"];
                    connectionStringInt = connectionStringInt.Replace("Provider=SQLNCLI10;", "")
                        .Replace("Provider=SQLNCLI11;", "").Replace("Provider=SQLNCLI12;", "");

                    var connectionStringPres = configList["connectionStringPresentation"];
                    connectionStringPres = connectionStringPres.Replace("Provider=SQLNCLI10;", "")
                        .Replace("Provider=SQLNCLI11;", "").Replace("Provider=SQLNCLI12;", "");

                    // These variables are used as global variables throughout the application
                    // They will be set once after startup

                    string value;
                    string lookUpValue;

                    lookUpValue = "SourceDatabase";
                    if (configList.TryGetValue(lookUpValue, out value))
                    {
                        FormBase.TeamConfigurationSettings.SourceDatabaseName = value;
                    }
                    else
                    {
                        returnCode.AppendLine("The key/value pair " + lookUpValue + " was not found in the configuration file.");
                    }

                    lookUpValue = "StagingDatabase";
                    if (configList.TryGetValue(lookUpValue, out value))
                    {
                        FormBase.TeamConfigurationSettings.StagingDatabaseName = value;
                    }
                    else
                    {
                        returnCode.AppendLine("The key/value pair " + lookUpValue + " was not found in the configuration file.");
                    }

                    lookUpValue = "PersistentStagingDatabase";
                    if (configList.TryGetValue(lookUpValue, out value))
                    {
                        FormBase.TeamConfigurationSettings.PsaDatabaseName = value;
                    }
                    else
                    {
                        returnCode.AppendLine("The key/value pair " + lookUpValue + " was not found in the configuration file.");
                    }

                    lookUpValue = "IntegrationDatabase";
                    if (configList.TryGetValue(lookUpValue, out value))
                    {
                        FormBase.TeamConfigurationSettings.IntegrationDatabaseName = value;
                    }
                    else
                    {
                        returnCode.AppendLine("The key/value pair " + lookUpValue + " was not found in the configuration file.");
                    }

                    lookUpValue = "PresentationDatabase";
                    if (configList.TryGetValue(lookUpValue, out value))
                    {
                        FormBase.TeamConfigurationSettings.PresentationDatabaseName = value;
                    }
                    else
                    {
                        returnCode.AppendLine("The key/value pair " + lookUpValue + " was not found in the configuration file.");
                    }

                    lookUpValue = "MetadataDatabase";
                    if (configList.TryGetValue(lookUpValue, out value))
                    {
                        FormBase.TeamConfigurationSettings.MetadataDatabaseName = value;
                    }
                    else
                    {
                        returnCode.AppendLine("The key/value pair " + lookUpValue + " was not found in the configuration file.");
                    }

                    lookUpValue = "PhysicalModelServerName";
                    if (configList.TryGetValue(lookUpValue, out value))
                    {
                        FormBase.TeamConfigurationSettings.PhysicalModelServerName = value;
                    }
                    else
                    {
                        returnCode.AppendLine("The key/value pair " + lookUpValue + " was not found in the configuration file.");
                    }

                    lookUpValue = "MetadataServerName";
                    if (configList.TryGetValue(lookUpValue, out value))
                    {
                        FormBase.TeamConfigurationSettings.MetadataServerName = value;
                    }
                    else
                    {
                        returnCode.AppendLine("The key/value pair " + lookUpValue + " was not found in the configuration file.");
                    }

                    //lookUpValue = connectionStringSource;
                    //if (configList.TryGetValue(lookUpValue, out value))
                    //{
                    //    FormBase.TeamConfigurationSettings.ConnectionStringSource = value;
                    //}
                    //else
                    //{
                    //    returnCode.AppendLine("The key/value pair " + lookUpValue + " was not found in the configuration file.");
                    //}

                    //lookUpValue = connectionStringStg;
                    //if (configList.TryGetValue(lookUpValue, out value))
                    //{
                    //    FormBase.TeamConfigurationSettings.ConnectionStringStg = value;
                    //}
                    //else
                    //{
                    //    returnCode.AppendLine("The key/value pair " + lookUpValue + " was not found in the configuration file.");
                    //}

                    // 10
                    FormBase.TeamConfigurationSettings.ConnectionStringSource = connectionStringSource;
                    FormBase.TeamConfigurationSettings.ConnectionStringStg = connectionStringStg;
                    FormBase.TeamConfigurationSettings.ConnectionStringHstg = connectionStringHstg;
                    FormBase.TeamConfigurationSettings.ConnectionStringInt = connectionStringInt;
                    FormBase.TeamConfigurationSettings.ConnectionStringOmd = connectionStringOmd;
                    FormBase.TeamConfigurationSettings.ConnectionStringPres = connectionStringPres;
                    
                    FormBase.TeamConfigurationSettings.StgTablePrefixValue = configList["StagingAreaPrefix"];
                    FormBase.TeamConfigurationSettings.PsaTablePrefixValue = configList["PersistentStagingAreaPrefix"];
                    FormBase.TeamConfigurationSettings.HubTablePrefixValue = configList["HubTablePrefix"];
                    FormBase.TeamConfigurationSettings.SatTablePrefixValue = configList["SatTablePrefix"];
                    FormBase.TeamConfigurationSettings.LinkTablePrefixValue = configList["LinkTablePrefix"];
                    // 20
                    FormBase.TeamConfigurationSettings.LsatPrefixValue = configList["LinkSatTablePrefix"];
                    FormBase.TeamConfigurationSettings.DwhKeyIdentifier = configList["KeyIdentifier"];
                    FormBase.TeamConfigurationSettings.SchemaName = configList["SchemaName"];
                    FormBase.TeamConfigurationSettings.RowIdAttribute = configList["RowID"];
                    FormBase.TeamConfigurationSettings.EventDateTimeAttribute = configList["EventDateTimeStamp"];
                    FormBase.TeamConfigurationSettings.LoadDateTimeAttribute = configList["LoadDateTimeStamp"];
                    FormBase.TeamConfigurationSettings.ExpiryDateTimeAttribute = configList["ExpiryDateTimeStamp"];
                    FormBase.TeamConfigurationSettings.ChangeDataCaptureAttribute = configList["ChangeDataIndicator"];
                    FormBase.TeamConfigurationSettings.RecordSourceAttribute = configList["RecordSourceAttribute"];
                    FormBase.TeamConfigurationSettings.EtlProcessAttribute = configList["ETLProcessID"];
                    // 30
                    FormBase.TeamConfigurationSettings.EtlProcessUpdateAttribute = configList["ETLUpdateProcessID"];
                    FormBase.TeamConfigurationSettings.LogicalDeleteAttribute = configList["LogicalDeleteAttribute"];
                    FormBase.TeamConfigurationSettings.TableNamingLocation = configList["TableNamingLocation"];
                    FormBase.TeamConfigurationSettings.KeyNamingLocation = configList["KeyNamingLocation"];
                    FormBase.TeamConfigurationSettings.RecordChecksumAttribute = configList["RecordChecksum"];
                    FormBase.TeamConfigurationSettings.CurrentRowAttribute = configList["CurrentRecordAttribute"];
                    FormBase.TeamConfigurationSettings.AlternativeRecordSourceAttribute = configList["AlternativeRecordSource"];
                    FormBase.TeamConfigurationSettings.AlternativeLoadDateTimeAttribute = configList["AlternativeHubLDTS"];
                    FormBase.TeamConfigurationSettings.EnableAlternativeRecordSourceAttribute = configList["AlternativeRecordSourceFunction"];
                    FormBase.TeamConfigurationSettings.EnableAlternativeLoadDateTimeAttribute = configList["AlternativeHubLDTSFunction"];
                    // 40
                    FormBase.TeamConfigurationSettings.EnableAlternativeSatelliteLoadDateTimeAttribute = configList["AlternativeSatelliteLDTSFunction"];
                    FormBase.TeamConfigurationSettings.AlternativeSatelliteLoadDateTimeAttribute = configList["AlternativeSatelliteLDTS"];
                    FormBase.TeamConfigurationSettings.PsaKeyLocation = configList["PSAKeyLocation"];
                    FormBase.TeamConfigurationSettings.MetadataRepositoryType = configList["metadataRepositoryType"];

                }
                else
                {
                    returnCode.AppendLine("An error was encountered: the file " + filename + " was not found.");
                }
            }
            catch (Exception ex)
            {
                returnCode.AppendLine("an exception was encountered attempting to load file '" + filename + "'. The error is:"+ex+".");
            }

            return returnCode;
        }
    }
}
