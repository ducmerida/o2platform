// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.IO;
using System.Drawing;
using System.Linq;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using O2.Kernel;
using O2.Kernel.Interfaces.O2Core;
using O2.DotNetWrappers.DotNet;
using O2.DotNetWrappers.Windows;
using O2.DotNetWrappers.Network;
using O2.Views.ASCX;
using O2.Views.ASCX.CoreControls;
using O2.Kernel.Interfaces.Views;
using O2.Views.ASCX.classes;
using O2.External.WinFormsUI.Forms;
using O2.External.SharpDevelop.Ascx;
using O2.Views.ASCX.MerlinWizard;
using O2.Views.ASCX.MerlinWizard.O2Wizard_ExtensionMethods;
using O2.Tool.SearchEngine.Ascx;
using O2.DotNetWrappers.Zip;
// extra references and the namespaces they import
//O2Tag_AddReferenceFile:nunit.framework.dll
using NUnit.Framework; 
//O2Ref:merlin.dll
using Merlin;
using MerlinStepLibrary;
//O2_File:C:\O2\O2 - All Active Projects\O2 - All Active Projects\O2Core\O2_Views_ASCX\MerlinWizard\MerlinWizard_ExtensionMethods_2.cs
//O2_File:C:\O2\O2 - All Active Projects\O2 - All Active Projects\O2Core\O2_Views_ASCX\MerlinWizard\O2Wizard.cs


namespace O2.Script
{	

/*	public class debugTest
	{
		public static void test()
		{
			new Workflow_SearchEngine_UseCase_1().runWizard();
		}
	}
*/
	[TestFixture]	
	public class Workflow_SearchEngine_UseCase_1
	{
		public static string testFolder = @"C:\_Dinis_localDevelop";
		private static IO2Log log = PublicDI.log;
		
        [Test]        
        public string runWizard()
        {
            return runWizard(testFolder);
        }

        public string runWizard(string startFolder)
        {                                     	
        	var o2Wizard = new O2Wizard("Search Engine - Use Case 1"); 

        	o2Wizard.Steps.add_Control(new ascx_FileMappings(),
        							  "Search Targets", 
        							  "Drag and Drop target folder with source files and Select files to index", step0_loadDefaultValues);        	
			o2Wizard.Steps.add_Action("Confirm files to index", confirmFilesToIndex);
        	o2Wizard.Steps.add_Control(typeof(ascx_SearchTargets),"Loading search targets", "Click next once loading is complete", step2_loadFiles);        	
        	o2Wizard.Steps.add_Control(typeof(ascx_SearchResults),"Search loaded targets", "Search Indexed files", step3_SearchResults);
        	        	         	        	         	
            o2Wizard.start();
            return "ok";
        }		
        
        public void step0_loadDefaultValues(IStep step)
        {
        	O2Thread.mtaThread(
        		() => 	{		
        					PublicDI.log.error("in step0_loadDefaultValues");
        					PublicDI.log.error(step.Controller.steps[0].FirstControl.GetType().FullName);
				        	var fileMappings = (ascx_FileMappings)step.Controller.steps[0].FirstControl;
				        	PublicDI.reflection.invoke(fileMappings,"hideDropHelpInfo", new object[]{});
				        	var fileFilter = ".cs";
				        	fileMappings.setExtensionsToShow(fileFilter);
				        	fileMappings.loadFilesFromFolder(PublicDI.config.O2TempDir,fileFilter);
			        	});			        
        }
        
        public void step2_loadFiles(IStep step)
        {
        	O2Thread.mtaThread(
        		() => 	{				        	
				        	var testFiles = Files.getFilesFromDir_returnFullPath(PublicDI.config.O2TempDir,"*.cs", true);
				        	var searchTargets = (ascx_SearchTargets)step.Controller.steps[2].FirstControl;
				        	searchTargets.loadFiles(testFiles);
				        });
        }
        
        public void step3_SearchResults(IStep step)
        {
        	var searchResults = (ascx_SearchResults)step.Controller.steps[3].FirstControl;
        	searchResults.setOpenSelectedItemInMainGUIWindowCheckedState(false);
        	searchResults.addSearchCriteria("test.*");
        }
        
        public void confirmFilesToIndex(IStep step)
        {
        	var fileMappings = (ascx_FileMappings)step.Controller.steps[0].FirstControl;        	
        	var filesToIndex = fileMappings.getFilesThatMatchCurrentExtensionFilter();
        	
        	step.setText(string.Format("There are {0} files to index", filesToIndex.Count));
        	var filesText = new StringBuilder();
        	filesText.AppendLine();
        	foreach(var file in filesToIndex)
        		filesText.AppendLine("  -  " + file);
			step.appendText(filesText.ToString());
       	}
	}		
}
