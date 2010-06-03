LoadAssembly("bin/FluentNHibernate.dll");
LoadAssembly("bin/Iesi.Collections.dll");
LoadAssembly("bin/LinFu.Core.dll");
LoadAssembly("bin/LinFu.DynamicProxy.dll");
LoadAssembly("bin/log4net.dll");
LoadAssembly("bin/Microsoft.Practices.ServiceLocation.dll");
LoadAssembly("bin/Mono.Security.dll");
LoadAssembly("bin/Newtonsoft.Json.dll");
LoadAssembly("bin/NHaml.Core.dll");
LoadAssembly("bin/NHaml.Web.Mvc.dll");
LoadAssembly("bin/NHibernate.dll");
LoadAssembly("bin/NHibernate.ByteCode.LinFu.dll");
LoadAssembly("bin/NHibernate.Validator.dll");
LoadAssembly("bin/Npgsql.dll");
LoadAssembly("bin/Shaml.Core.dll");
LoadAssembly("bin/Shaml.Core.Validator.dll");
LoadAssembly("bin/Shaml.Data.dll");
LoadAssembly("bin/Shaml.Membership.dll");
LoadAssembly("bin/Shaml.Testing.dll");
LoadAssembly("bin/Shaml.Tests.dll");
LoadAssembly("bin/Shaml.Web.dll");
LoadAssembly("bin/CodeChirp.dll");
LoadAssembly("bin/CodeChirp.ApplicationServices.dll");
LoadAssembly("bin/CodeChirp.Config.dll");
LoadAssembly("bin/CodeChirp.Controllers.dll");
LoadAssembly("bin/CodeChirp.Core.dll");
LoadAssembly("bin/CodeChirp.Data.dll");
LoadAssembly("bin/CodeChirp.Tests.dll");

using System;
using System.IO;
using System.Collections.Generic;
using CodeChirp.Data.Mapping;
using CodeChirp.Core;
using Shaml.Membership.Core;
using Shaml.Data.NHibernate;
using Shaml.Testing.NHibernate;
using NHibernate;
using NHibernate.Metadata;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using NHibernate.Criterion;

Configuration configuration;
string[] mappingAssemblies = new string[1];
mappingAssemblies[0] = "bin/CodeChirp.Data.dll";
configuration = NHibernateSession.Init(
  new SimpleSessionStorage(), mappingAssemblies,
  new AutoPersistenceModelGenerator().Generate(),
  "Config/NHibernate.config");
CodeChirp.Config.ComponentRegistrar.InitializeServiceLocator();
var s = NHibernateSession.GetDefaultSessionFactory().OpenSession();

