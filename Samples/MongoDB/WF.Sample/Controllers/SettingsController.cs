﻿using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using WF.Sample.Business;
using WF.Sample.Models;
using WF.Sample.Business.Helpers;
using WF.Sample.Business.Models;
using WF.Sample.Business.Workflow;
using OptimaJet.Workflow.MongoDB;
using MongoDB.Driver;
using MongoDB.Bson;

namespace WF.Sample.Controllers
{
    public class SettingsController : Controller
    {
        public ActionResult Edit()
        {

            return View(GetModel());
        }

        const string SchemeName = "SimpleWF";

        public ActionResult GenerateData()
        {
            var roles = new List<Role>() { 
                    new Role(){Id = new Guid("8D378EBE-0666-46B3-B7AB-1A52480FD12A"), Name = "Big Boss" },
                    new Role(){Id = new Guid("412174C2-0490-4101-A7B3-830DE90BCAA0"), Name = "Accountant"},
                    new Role(){Id = new Guid("71FFFB5B-B707-4B3C-951C-C37FDFCC8DFB"), Name = "User"}
                };

            var sd = new List<StructDivision>(){
                   new StructDivision(){Id = new Guid("B14F5D81-5B0D-4ACC-92B8-27CBBE39086B"), Name = "Group 1", ParentId = new Guid("F6E34BDF-B769-42DD-A2BE-FEE67FAF9045")},
                   new StructDivision(){Id = new Guid("7E9FD972-C775-4C6B-9D91-47E9397BD2E6"), Name = "Group 1.1", ParentId = new Guid("B14F5D81-5B0D-4ACC-92B8-27CBBE39086B")},
                   new StructDivision(){Id = new Guid("DC195A4F-46F9-41B2-80D2-77FF9C6269B7"), Name = "Group 1.2", ParentId = new Guid("B14F5D81-5B0D-4ACC-92B8-27CBBE39086B")},
                   new StructDivision(){Id = new Guid("C5DCC148-9C0C-45C4-8A68-901D99A26184"), Name = "Group 2.2", ParentId = new Guid("72D461B2-234B-40D6-B410-B261964BA291")},
                   new StructDivision(){Id = new Guid("72D461B2-234B-40D6-B410-B261964BA291"), Name = "Group 2", ParentId = new Guid("F6E34BDF-B769-42DD-A2BE-FEE67FAF9045")},
                   new StructDivision(){Id = new Guid("BC21A482-28E7-4951-8177-E57813A70FC5"), Name = "Group 2.1", ParentId = new Guid("72D461B2-234B-40D6-B410-B261964BA291")},
		           new StructDivision(){Id = new Guid("F6E34BDF-B769-42DD-A2BE-FEE67FAF9045"), Name = "Head Group"}
                };

            var e1 = new Employee()
            {
                Id = new Guid("E41B48E3-C03D-484F-8764-1711248C4F8A"),
                Name = "Maria",
                StructDivisionId = new Guid("C5DCC148-9C0C-45C4-8A68-901D99A26184"),
                IsHead = false
            };

            e1.Roles.Add(new Guid("412174C2-0490-4101-A7B3-830DE90BCAA0"), "Accountant");
            e1.Roles.Add(new Guid("71FFFB5B-B707-4B3C-951C-C37FDFCC8DFB"), "User");

            var e2 = new Employee()
            {
                Id = new Guid("BBE686F8-8736-48A7-A886-2DA25567F978"),
                Name = "Mark",
                StructDivisionId = new Guid("7E9FD972-C775-4C6B-9D91-47E9397BD2E6"),
                IsHead = false,
            };
            e2.Roles.Add(new Guid("71FFFB5B-B707-4B3C-951C-C37FDFCC8DFB"), "User");

            var e3 = new Employee()
            {
                Id = new Guid("81537E21-91C5-4811-A546-2DDDFF6BF409"),
                Name = "Silviya",
                StructDivisionId = new Guid("F6E34BDF-B769-42DD-A2BE-FEE67FAF9045"),
                IsHead = true,
            };
            e3.Roles.Add(new Guid("8D378EBE-0666-46B3-B7AB-1A52480FD12A"), "Big Boss");
            e3.Roles.Add(new Guid("71FFFB5B-B707-4B3C-951C-C37FDFCC8DFB"), "User");

            var e4 = new Employee()
            {
                Id = new Guid("B0E6FD4C-2DB9-4BB6-A62E-68B6B8999905"),
                Name = "Margo",
                StructDivisionId = new Guid("DC195A4F-46F9-41B2-80D2-77FF9C6269B7"),
                IsHead = false,
            };
            e4.Roles.Add(new Guid("71FFFB5B-B707-4B3C-951C-C37FDFCC8DFB"), "User");

            var e5 = new Employee()
            {
                Id = new Guid("DEB579F9-991C-4DB9-A17D-BB1ECCF2842C"),
                Name = "Max",
                StructDivisionId = new Guid("B14F5D81-5B0D-4ACC-92B8-27CBBE39086B"),
                IsHead = true,
            };
            e5.Roles.Add(new Guid("71FFFB5B-B707-4B3C-951C-C37FDFCC8DFB"), "User");

            var e6 = new Employee()
            {
                Id = new Guid("91F2B471-4A96-4AB7-A41A-EA4293703D16"),
                Name = "John",
                StructDivisionId = new Guid("7E9FD972-C775-4C6B-9D91-47E9397BD2E6"),
                IsHead = true,
            };

            e6.Roles.Add(new Guid("71FFFB5B-B707-4B3C-951C-C37FDFCC8DFB"), "User");
            var employees = new List<Employee>() { e1 , e2, e3, e4, e5, e6 };


            var dbcollRole = WorkflowInit.Provider.Store.GetCollection<Role>("Role");
            foreach (var r in roles)
            {
                if (dbcollRole.Find(x => x.Id == r.Id).FirstOrDefault() == null)
                    dbcollRole.InsertOne(r);
            }

            var dbcollStructDivision = WorkflowInit.Provider.Store.GetCollection<StructDivision>("StructDivision");
            foreach (var e in sd)
            {
                if (dbcollStructDivision.Find(x => x.Id == e.Id).FirstOrDefault() == null)
                    dbcollStructDivision.InsertOne(e);
            }

            var dbcollEmployee = WorkflowInit.Provider.Store.GetCollection<Employee>("Employee");
            foreach (var e in employees)
            {
                if (dbcollEmployee.Find(x => x.Id == e.Id).FirstOrDefault() == null)
                {
                    e.StructDivisionName = sd.Where(c => c.Id == e.StructDivisionId).First().Name;
                    dbcollEmployee.InsertOne(e);
                }
            }

            var dbcollWorkflowScheme = WorkflowInit.Provider.Store.GetCollection<WorkflowScheme>("WorkflowScheme");

            if (dbcollWorkflowScheme.Find(x => x.Id == SchemeName).FirstOrDefault() == null)
            {
                #region Insert scheme
                dbcollWorkflowScheme.InsertOne(new WorkflowScheme()
                {
                    Id = SchemeName,
                    Code = SchemeName,
                    Scheme = @"<Process Name=""SimpleWF"">
  <Designer X=""-110"" Y=""-60"" />
  <Actors>
    <Actor Name=""Author"" Rule=""IsDocumentAuthor"" Value="""" />
    <Actor Name=""AuthorsBoss"" Rule=""IsAuthorsBoss"" Value="""" />
    <Actor Name=""Controller"" Rule=""IsDocumentController"" Value="""" />
    <Actor Name=""BigBoss"" Rule=""CheckRole"" Value=""Big Boss"" />
    <Actor Name=""Accountant"" Rule=""CheckRole"" Value=""Accountant"" />
  </Actors>
  <Parameters>
    <Parameter Name=""Comment"" Type=""System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"" Purpose=""Temporary"" />
  </Parameters>
  <Commands>
    <Command Name=""StartProcessing"">
      <InputParameters>
        <ParameterRef Name=""Comment"" NameRef=""Comment"" />
      </InputParameters>
    </Command>
    <Command Name=""Sighting"">
      <InputParameters>
        <ParameterRef Name=""Comment"" NameRef=""Comment"" />
      </InputParameters>
    </Command>
    <Command Name=""Denial"">
      <InputParameters>
        <ParameterRef Name=""Comment"" NameRef=""Comment"" />
      </InputParameters>
    </Command>
    <Command Name=""Paid"">
      <InputParameters>
        <ParameterRef Name=""Comment"" NameRef=""Comment"" />
      </InputParameters>
    </Command>
  </Commands>
  <Timers>
    <Timer Name=""ControllerTimer"" Type=""Interval"" Value=""120000"" NotOverrideIfExists=""false"" />
  </Timers>
  <Activities>
    <Activity Name=""DraftInitial"" State=""Draft"" IsInitial=""True"" IsFinal=""False"" IsForSetState=""False"" IsAutoSchemeUpdate=""False"">
      <PreExecutionImplementation>
        <ActionRef Order=""1"" NameRef=""WriteTransitionHistory""/>
      </PreExecutionImplementation>
      <Designer X=""40"" Y=""80""/>
    </Activity>
    <Activity Name=""Draft"" State=""Draft"" IsInitial=""False"" IsFinal=""False"" IsForSetState=""True"" IsAutoSchemeUpdate=""True"">
      <Implementation>
        <ActionRef Order=""1"" NameRef=""UpdateTransitionHistory""/>
      </Implementation>
      <PreExecutionImplementation>
        <ActionRef Order=""1"" NameRef=""WriteTransitionHistory""/>
      </PreExecutionImplementation>
      <Designer X=""450"" Y=""390""/>
    </Activity>
    <Activity Name=""DraftStartProcessingExecute"" IsInitial=""False"" IsFinal=""False"" IsForSetState=""False"" IsAutoSchemeUpdate=""False"">
      <Designer X=""40"" Y=""240""/>
    </Activity>
    <Activity Name=""ControllerSighting"" State=""ControllerSighting"" IsInitial=""False"" IsFinal=""False"" IsForSetState=""True"" IsAutoSchemeUpdate=""True"">
      <Implementation>
        <ActionRef Order=""1"" NameRef=""UpdateTransitionHistory""/>
      </Implementation>
      <PreExecutionImplementation>
        <ActionRef Order=""1"" NameRef=""WriteTransitionHistory""/>
      </PreExecutionImplementation>
      <Designer X=""310"" Y=""240""/>
    </Activity>
    <Activity Name=""ControllerSightingExecute"" IsInitial=""False"" IsFinal=""False"" IsForSetState=""False"" IsAutoSchemeUpdate=""False"">
      <Designer X=""310"" Y=""80""/>
    </Activity>
    <Activity Name=""AuthorBossSighting"" State=""AuthorBossSighting"" IsInitial=""False"" IsFinal=""False"" IsForSetState=""True"" IsAutoSchemeUpdate=""True"">
      <Implementation>
        <ActionRef Order=""1"" NameRef=""UpdateTransitionHistory""/>
      </Implementation>
      <PreExecutionImplementation>
        <ActionRef Order=""1"" NameRef=""WriteTransitionHistory""/>
      </PreExecutionImplementation>
      <Designer X=""620"" Y=""80""/>
    </Activity>
    <Activity Name=""AuthorConfirmation"" State=""AuthorConfirmation"" IsInitial=""False"" IsFinal=""False"" IsForSetState=""True"" IsAutoSchemeUpdate=""True"">
      <Implementation>
        <ActionRef Order=""1"" NameRef=""UpdateTransitionHistory""/>
      </Implementation>
      <PreExecutionImplementation>
        <ActionRef Order=""1"" NameRef=""WriteTransitionHistory""/>
      </PreExecutionImplementation>
      <Designer X=""600"" Y=""240""/>
    </Activity>
    <Activity Name=""BigBossSighting"" State=""BigBossSighting"" IsInitial=""False"" IsFinal=""False"" IsForSetState=""True"" IsAutoSchemeUpdate=""True"">
      <Implementation>
        <ActionRef Order=""1"" NameRef=""UpdateTransitionHistory""/>
      </Implementation>
      <PreExecutionImplementation>
        <ActionRef Order=""1"" NameRef=""WriteTransitionHistory""/>
      </PreExecutionImplementation>
      <Designer X=""890"" Y=""390""/>
    </Activity>
    <Activity Name=""AccountantProcessing"" State=""AccountantProcessing"" IsInitial=""False"" IsFinal=""False"" IsForSetState=""True"" IsAutoSchemeUpdate=""True"">
      <Implementation>
        <ActionRef Order=""1"" NameRef=""UpdateTransitionHistory""/>
      </Implementation>
      <PreExecutionImplementation>
        <ActionRef Order=""1"" NameRef=""WriteTransitionHistory""/>
      </PreExecutionImplementation>
      <Designer X=""890"" Y=""240""/>
    </Activity>
    <Activity Name=""Paid"" State=""Paid"" IsInitial=""False"" IsFinal=""True"" IsForSetState=""True"" IsAutoSchemeUpdate=""True"">
      <Implementation>
        <ActionRef Order=""1"" NameRef=""UpdateTransitionHistory""/>
      </Implementation>
      <PreExecutionImplementation>
        <ActionRef Order=""1"" NameRef=""WriteTransitionHistory""/>
      </PreExecutionImplementation>
      <Designer X=""880"" Y=""80""/>
    </Activity>
  </Activities>
  <Transitions>
    <Transition Name=""DraftInitial"" To=""DraftStartProcessingExecute"" From=""DraftInitial"" Classifier=""Direct"" AllowConcatenationType=""And"" RestrictConcatenationType=""And"" ConditionsConcatenationType=""And"" IsFork=""false"" MergeViaSetState=""false"" DisableParentStateControl=""false"">
      <Restrictions>
        <Restriction Type=""Allow"" NameRef=""Author""/>
      </Restrictions>
      <Triggers>
        <Trigger Type=""Command"" NameRef=""StartProcessing""/>
      </Triggers>
      <Conditions>
        <Condition Type=""Always""/>
      </Conditions>
      <Designer/>
    </Transition>
    <Transition Name=""Draft"" To=""ControllerSighting"" From=""Draft"" Classifier=""Direct"" AllowConcatenationType=""And"" RestrictConcatenationType=""And"" ConditionsConcatenationType=""And"" IsFork=""false"" MergeViaSetState=""false"" DisableParentStateControl=""false"">
      <Restrictions>
        <Restriction Type=""Allow"" NameRef=""Author""/>
      </Restrictions>
      <Triggers>
        <Trigger Type=""Command"" NameRef=""StartProcessing""/>
      </Triggers>
      <Conditions>
        <Condition Type=""Always""/>
      </Conditions>
      <Designer X=""470.5"" Y=""346""/>
    </Transition>
    <Transition Name=""DraftStartProcessingExecute_1"" To=""ControllerSighting"" From=""DraftStartProcessingExecute"" Classifier=""Direct"" AllowConcatenationType=""And"" RestrictConcatenationType=""And"" ConditionsConcatenationType=""And"" IsFork=""false"" MergeViaSetState=""false"" DisableParentStateControl=""false"">
      <Triggers>
        <Trigger Type=""Auto""/>
      </Triggers>
      <Conditions>
        <Condition Type=""Action"" NameRef=""CheckDocumentHasController"" ConditionInversion=""false""/>
      </Conditions>
      <Designer X=""263.5"" Y=""270""/>
    </Transition>
    <Transition Name=""DraftStartProcessingExecute_2"" To=""ControllerSightingExecute"" From=""DraftStartProcessingExecute"" Classifier=""Direct"" AllowConcatenationType=""And"" RestrictConcatenationType=""And"" ConditionsConcatenationType=""And"" IsFork=""false"" MergeViaSetState=""false"" DisableParentStateControl=""false"">
      <Triggers>
        <Trigger Type=""Auto""/>
      </Triggers>
      <Conditions>
        <Condition Type=""Otherwise""/>
      </Conditions>
      <Designer X=""265"" Y=""188""/>
    </Transition>
    <Transition Name=""ControllerSighting"" To=""ControllerSightingExecute"" From=""ControllerSighting"" Classifier=""Direct"" AllowConcatenationType=""And"" RestrictConcatenationType=""And"" ConditionsConcatenationType=""And"" IsFork=""false"" MergeViaSetState=""false"" DisableParentStateControl=""false"">
      <Restrictions>
        <Restriction Type=""Allow"" NameRef=""Controller""/>
      </Restrictions>
      <Triggers>
        <Trigger Type=""Command"" NameRef=""Sighting""/>
      </Triggers>
      <Conditions>
        <Condition Type=""Always""/>
      </Conditions>
      <Designer X=""373"" Y=""190.5""/>
    </Transition>
    <Transition Name=""ControllerSighting_R"" To=""Draft"" From=""ControllerSighting"" Classifier=""Reverse"" AllowConcatenationType=""And"" RestrictConcatenationType=""And"" ConditionsConcatenationType=""And"" IsFork=""false"" MergeViaSetState=""false"" DisableParentStateControl=""false"">
      <Restrictions>
        <Restriction Type=""Allow"" NameRef=""Controller""/>
      </Restrictions>
      <Triggers>
        <Trigger Type=""Command"" NameRef=""Denial""/>
      </Triggers>
      <Conditions>
        <Condition Type=""Always""/>
      </Conditions>
      <Designer X=""380.5"" Y=""354.5""/>
    </Transition>
    <Transition Name=""ControllerSightingExecute_1"" To=""AuthorConfirmation"" From=""ControllerSightingExecute"" Classifier=""Direct"" AllowConcatenationType=""And"" RestrictConcatenationType=""And"" ConditionsConcatenationType=""And"" IsFork=""false"" MergeViaSetState=""false"" DisableParentStateControl=""false"">
      <Triggers>
        <Trigger Type=""Auto""/>
      </Triggers>
      <Conditions>
        <Condition Type=""Action"" NameRef=""CheckDocumentsAuthorIsBoss"" ConditionInversion=""false""/>
      </Conditions>
      <Designer X=""528.5"" Y=""177""/>
    </Transition>
    <Transition Name=""ControllerSightingExecute_2"" To=""AuthorBossSighting"" From=""ControllerSightingExecute"" Classifier=""Direct"" AllowConcatenationType=""And"" RestrictConcatenationType=""And"" ConditionsConcatenationType=""And"" IsFork=""false"" MergeViaSetState=""false"" DisableParentStateControl=""false"">
      <Triggers>
        <Trigger Type=""Auto""/>
      </Triggers>
      <Conditions>
        <Condition Type=""Otherwise""/>
      </Conditions>
      <Designer X=""558.5"" Y=""101""/>
    </Transition>
    <Transition Name=""AuthorBossSighting"" To=""AuthorConfirmation"" From=""AuthorBossSighting"" Classifier=""Direct"" AllowConcatenationType=""And"" RestrictConcatenationType=""And"" ConditionsConcatenationType=""And"" IsFork=""false"" MergeViaSetState=""false"" DisableParentStateControl=""false"">
      <Restrictions>
        <Restriction Type=""Allow"" NameRef=""AuthorsBoss""/>
      </Restrictions>
      <Triggers>
        <Trigger Type=""Command"" NameRef=""Sighting""/>
      </Triggers>
      <Conditions>
        <Condition Type=""Always""/>
      </Conditions>
      <Designer X=""692"" Y=""190""/>
    </Transition>
    <Transition Name=""AuthorBossSighting_R"" To=""Draft"" From=""AuthorBossSighting"" Classifier=""Reverse"" AllowConcatenationType=""And"" RestrictConcatenationType=""And"" ConditionsConcatenationType=""And"" IsFork=""false"" MergeViaSetState=""false"" DisableParentStateControl=""false"">
      <Restrictions>
        <Restriction Type=""Allow"" NameRef=""AuthorsBoss""/>
      </Restrictions>
      <Triggers>
        <Trigger Type=""Command"" NameRef=""Denial""/>
      </Triggers>
      <Conditions>
        <Condition Type=""Always""/>
      </Conditions>
      <Designer X=""560"" Y=""274""/>
    </Transition>
    <Transition Name=""AuthorConfirmation_1"" To=""BigBossSighting"" From=""AuthorConfirmation"" Classifier=""Direct"" AllowConcatenationType=""And"" RestrictConcatenationType=""And"" ConditionsConcatenationType=""And"" IsFork=""false"" MergeViaSetState=""false"" DisableParentStateControl=""false"">
      <Restrictions>
        <Restriction Type=""Allow"" NameRef=""Author""/>
      </Restrictions>
      <Triggers>
        <Trigger Type=""Command"" NameRef=""Sighting""/>
      </Triggers>
      <Conditions>
        <Condition Type=""Action"" NameRef=""CheckBigBossMustSight"" ConditionInversion=""false""/>
      </Conditions>
      <Designer X=""755.5"" Y=""345.5""/>
    </Transition>
    <Transition Name=""AuthorConfirmation_2"" To=""AccountantProcessing"" From=""AuthorConfirmation"" Classifier=""Direct"" AllowConcatenationType=""And"" RestrictConcatenationType=""And"" ConditionsConcatenationType=""And"" IsFork=""false"" MergeViaSetState=""false"" DisableParentStateControl=""false"">
      <Restrictions>
        <Restriction Type=""Allow"" NameRef=""Author""/>
      </Restrictions>
      <Triggers>
        <Trigger Type=""Command"" NameRef=""Sighting""/>
      </Triggers>
      <Conditions>
        <Condition Type=""Otherwise""/>
      </Conditions>
      <Designer X=""840"" Y=""251""/>
    </Transition>
    <Transition Name=""AuthorConfirmation_R"" To=""Draft"" From=""AuthorConfirmation"" Classifier=""Reverse"" AllowConcatenationType=""And"" RestrictConcatenationType=""And"" ConditionsConcatenationType=""And"" IsFork=""false"" MergeViaSetState=""false"" DisableParentStateControl=""false"">
      <Restrictions>
        <Restriction Type=""Allow"" NameRef=""Author""/>
      </Restrictions>
      <Triggers>
        <Trigger Type=""Command"" NameRef=""Denial""/>
      </Triggers>
      <Conditions>
        <Condition Type=""Always""/>
      </Conditions>
      <Designer/>
    </Transition>
    <Transition Name=""BigBossSighting"" To=""AccountantProcessing"" From=""BigBossSighting"" Classifier=""Direct"" AllowConcatenationType=""And"" RestrictConcatenationType=""And"" ConditionsConcatenationType=""And"" IsFork=""false"" MergeViaSetState=""false"" DisableParentStateControl=""false"">
      <Restrictions>
        <Restriction Type=""Allow"" NameRef=""BigBoss""/>
      </Restrictions>
      <Triggers>
        <Trigger Type=""Command"" NameRef=""Sighting""/>
      </Triggers>
      <Conditions>
        <Condition Type=""Always""/>
      </Conditions>
      <Designer X=""982"" Y=""351.5""/>
    </Transition>
    <Transition Name=""BigBossSighting_R"" To=""Draft"" From=""BigBossSighting"" Classifier=""Reverse"" AllowConcatenationType=""And"" RestrictConcatenationType=""And"" ConditionsConcatenationType=""And"" IsFork=""false"" MergeViaSetState=""false"" DisableParentStateControl=""false"">
      <Restrictions>
        <Restriction Type=""Allow"" NameRef=""BigBoss""/>
      </Restrictions>
      <Triggers>
        <Trigger Type=""Command"" NameRef=""Denial""/>
      </Triggers>
      <Conditions>
        <Condition Type=""Always""/>
      </Conditions>
      <Designer X=""765"" Y=""431""/>
    </Transition>
    <Transition Name=""AccountantProcessing"" To=""Paid"" From=""AccountantProcessing"" Classifier=""Direct"" AllowConcatenationType=""And"" RestrictConcatenationType=""And"" ConditionsConcatenationType=""And"" IsFork=""false"" MergeViaSetState=""false"" DisableParentStateControl=""false"">
      <Restrictions>
        <Restriction Type=""Allow"" NameRef=""Accountant""/>
      </Restrictions>
      <Triggers>
        <Trigger Type=""Command"" NameRef=""Paid""/>
      </Triggers>
      <Conditions>
        <Condition Type=""Always""/>
      </Conditions>
      <Designer X=""980"" Y=""196.5""/>
    </Transition>
    <Transition Name=""AccountantProcessing_R"" To=""AuthorConfirmation"" From=""AccountantProcessing"" Classifier=""Reverse"" AllowConcatenationType=""And"" RestrictConcatenationType=""And"" ConditionsConcatenationType=""And"" IsFork=""false"" MergeViaSetState=""false"" DisableParentStateControl=""false"">
      <Restrictions>
        <Restriction Type=""Allow"" NameRef=""Accountant""/>
      </Restrictions>
      <Triggers>
        <Trigger Type=""Command"" NameRef=""Denial""/>
      </Triggers>
      <Conditions>
        <Condition Type=""Always""/>
      </Conditions>
      <Designer X=""842"" Y=""290""/>
    </Transition>
    <Transition Name=""ControllerSighting_ControllerSightingExecute_1"" To=""ControllerSightingExecute"" From=""ControllerSighting"" Classifier=""NotSpecified"" AllowConcatenationType=""And"" RestrictConcatenationType=""And"" ConditionsConcatenationType=""And"" IsFork=""false"" MergeViaSetState=""false"" DisableParentStateControl=""false"">
      <Triggers>
        <Trigger Type=""Timer"" NameRef=""ControllerTimer""/>
      </Triggers>
      <Conditions>
        <Condition Type=""Always""/>
      </Conditions>
      <Designer X=""433"" Y=""190""/>
    </Transition>
  </Transitions>
  <CodeActions>
  <CodeAction Name=""CheckDocumentHasController"" Type=""Condition"" IsGlobal=""False"">
      <ActionCode><![CDATA[var conditionResult = false;
var dbcoll = WorkflowInit.Provider.Store.GetCollection<Document>(""Document"");
var doc = dbcoll.Find(x => x.Id == processInstance.ProcessId).FirstOrDefault();
if (doc != null)
    conditionResult = doc.EmloyeeControlerId.HasValue;
return conditionResult;
]]></ActionCode>
      <Usings><![CDATA[System;System.Collections;System.Collections.Generic;System.Linq;OptimaJet.Workflow;OptimaJet.Workflow.Core.Model;WF.Sample.Business;WF.Sample.Business.Helpers;WF.Sample.Business.Properties;WF.Sample.Business.Workflow;WF.Sample.Business.Models;MongoDB.Driver;]]></Usings>
    </CodeAction>
    <CodeAction Name=""CheckDocumentsAuthorIsBoss"" Type=""Condition"" IsGlobal=""False"">
      <ActionCode><![CDATA[var conditionResult = false;
var dbcoll = WorkflowInit.Provider.Store.GetCollection<Document>(""Document"");
var doc = dbcoll.Find(x => x.Id == processInstance.ProcessId).FirstOrDefault();
if (doc != null)
    {
        var dbcollEmployee = WorkflowInit.Provider.Store.GetCollection<Employee>(""Employee"");
        var emp = dbcollEmployee.Find(x => x.Id == doc.AuthorId).FirstOrDefault();
        if (emp != null)
            conditionResult = emp.IsHead;
    }
return conditionResult;
]]></ActionCode>
      <Usings><![CDATA[System;System.Collections;System.Collections.Generic;System.Linq;OptimaJet.Workflow;OptimaJet.Workflow.Core.Model;WF.Sample.Business;WF.Sample.Business.Helpers;WF.Sample.Business.Properties;WF.Sample.Business.Workflow;WF.Sample.Business.Models;MongoDB.Driver;]]></Usings>
    </CodeAction>
    <CodeAction Name=""CheckBigBossMustSight"" Type=""Condition"" IsGlobal=""False"">
      <ActionCode><![CDATA[var conditionResult = false;
var dbcoll = WorkflowInit.Provider.Store.GetCollection<Document>(""Document"");
var doc = dbcoll.Find(x => x.Id == processInstance.ProcessId).FirstOrDefault();
if (doc != null)
    conditionResult = doc.Sum > 100;  
    return conditionResult;
]]></ActionCode>
      <Usings><![CDATA[System;System.Collections;System.Collections.Generic;System.Linq;OptimaJet.Workflow;OptimaJet.Workflow.Core.Model;WF.Sample.Business;WF.Sample.Business.Helpers;WF.Sample.Business.Properties;WF.Sample.Business.Workflow;WF.Sample.Business.Models;MongoDB.Driver;]]></Usings>
    </CodeAction>
   </CodeActions>
  <Localization>
    <Localize Type=""State"" IsDefault=""True"" Culture=""en-US"" ObjectName=""ControllerSighting"" Value=""Controller sighting"" />
    <Localize Type=""State"" IsDefault=""True"" Culture=""en-US"" ObjectName=""AuthorBossSighting"" Value=""Author's boss sighting"" />
    <Localize Type=""State"" IsDefault=""True"" Culture=""en-US"" ObjectName=""AuthorConfirmation"" Value=""Author confirmation"" />
    <Localize Type=""State"" IsDefault=""True"" Culture=""en-US"" ObjectName=""BigBossSighting"" Value=""BigBoss sighting"" />
    <Localize Type=""State"" IsDefault=""True"" Culture=""en-US"" ObjectName=""AccountantProcessing"" Value=""Accountant processing"" />
    <Localize Type=""Command"" IsDefault=""True"" Culture=""en-US"" ObjectName=""StartProcessing"" Value=""Start processing"" />
  </Localization>
</Process>"
                });
                #endregion
            }

            return RedirectToAction("Edit");
        }

        #region Other
        private SettingsModel GetModel()
        {
            var model = new SettingsModel();
            model.SchemeName = "SimpleWF";

            model.Employees = EmployeeHelper.GetAll();
            model.Roles = WorkflowInit.Provider.Store.GetCollection<Role>("Role").Find(new BsonDocument()).ToList();
            model.StructDivision = WorkflowInit.Provider.Store.GetCollection<StructDivision>("StructDivision").Find(new BsonDocument()).ToList();
            return model;
        }

        public static string GenerateColumnHtml(string name, StructDivision m, List<StructDivision> Model, List<Employee> employes, ref int index, string refId)
        {
            string valuePrefix = string.Format("{0}[{1}]", name, index);

            var sb = new StringBuilder();
            string trName = string.Format("tr_{0}{1}", name, index);

            sb.AppendFormat("<tr Id='{0}' {1}>", trName,
                            string.IsNullOrEmpty(refId) ? string.Empty : string.Format("class='child-of-{0}'", refId));
            sb.AppendFormat("<input type='hidden' name='{0}.Id' value='{1}'></input>", valuePrefix, m.Id);
            sb.AppendFormat("<input type='hidden' name='{0}.ParentId' value='{1}'></input>", valuePrefix, m.ParentId);
            sb.AppendFormat("<td class='columnTree'><b>{0}</b></td>", m.Name);
            sb.AppendFormat("<td></td>");
            sb.Append("</tr>");

            foreach (var item in employes.Where(c => c.StructDivisionId == m.Id))
            {
                index++;
                sb.Append(GenerateColumnHtml(name, item, ref index, trName));
            }

            foreach (var item in Model.Where(c => c.ParentId == m.Id))
            {
                index++;
                sb.Append(GenerateColumnHtml(name, item, Model, employes, ref index, trName));
            }

            return sb.ToString();
        }

        public static string GenerateColumnHtml(string name, Employee m, ref int index, string refId)
        {
            string valuePrefix = string.Format("{0}[{1}]", name, index);

            var sb = new StringBuilder();
            string trName = string.Format("tr_{0}{1}", name, index);

            sb.AppendFormat("<tr Id='{0}' {1}>", trName,
                            string.IsNullOrEmpty(refId) ? string.Empty : string.Format("class='child-of-{0}'", refId));
            sb.AppendFormat("<input type='hidden' name='{0}.Id' value='{1}'></input>", valuePrefix, m.Id);
            sb.AppendFormat("<td class='columnTree'>");
            sb.AppendFormat("{0}", m.Name);
            if (m.IsHead)
                sb.Append(" <b>Head</b>");
            sb.AppendFormat("</td>");
            sb.AppendFormat("<td>");
            sb.AppendFormat("{0}", string.Join(",", m.Roles.Select(c => c.Value).ToArray()));
            sb.AppendFormat("</td>");
            sb.Append("</tr>");

            return sb.ToString();
        }
        #endregion
    }
}
