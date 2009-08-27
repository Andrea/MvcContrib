using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using MvcContrib.TestHelper;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Assert=NUnit.Framework.Assert;
using AssertionException=MvcContrib.TestHelper.AssertionException;

namespace MvcContrib.UnitTests.TestHelper
{
    [TestFixture]
    public class RouteTestingExtensionsTester
    {
        public class FunkyController : Controller
        {
            public ActionResult Index()
            {
                return null;
            }

            public ActionResult Bar(string id)
            {
                return null;
            }

            public ActionResult New()
            {
                return null;
            }
			
			public ActionResult List(Bar bar)
			{
				return null;
			}

        	public ActionResult Foo(int id)
        	{
        		return null;
        	}
        }
		public class Bar
		{
			
		}

        public class AwesomeController : Controller
        {
        }

        [SetUp]
        public void Setup()
        {
            RouteTable.Routes.Clear();
            RouteTable.Routes.IgnoreRoute("{resource}.gif/{*pathInfo}");
            RouteTable.Routes.MapRoute(
                "default",
                "{controller}/{action}/{id}", 
                new { controller = "Funky", Action = "Index", id ="" });
        }

        [TearDown]
        public void TearDown()
        {
            RouteTable.Routes.Clear();
        }

        [Test]
        public void should_be_able_to_pull_routedata_from_a_string()
        {
            var routeData = "~/charlie/brown".Route();
            Assert.That(routeData, Is.Not.Null);

            Assert.That(routeData.Values.ContainsValue("charlie"));
            Assert.That(routeData.Values.ContainsValue("brown"));
        }

        [Test]
        public void should_be_able_to_match_controller_from_route_data()
        {
            "~/".Route().ShouldMapTo<FunkyController>();            
        }

        [Test, ExpectedException(typeof(AssertionException))]
        public void should_be_able_to_detect_when_a_controller_doesnt_match()
        {
            "~/".Route().ShouldMapTo<AwesomeController>();            
        }

        [Test]
        public void should_be_able_to_match_action_with_lambda()
        {
            "~/".Route().ShouldMapTo<FunkyController>(x => x.Index());
        }

        [Test, ExpectedException(typeof(AssertionException))]
        public void should_be_able_to_detect_an_incorrect_action()
        {
            "~/".Route().ShouldMapTo<FunkyController>(x=>x.New());
        }

        [Test]
        public void should_be_able_to_match_action_parameters()
        {
            "~/funky/bar/widget".Route().ShouldMapTo<FunkyController>(x => x.Bar("widget"));
        }

        [Test, ExpectedException(typeof(AssertionException))]
        public void should_be_able_to_detect_invalid_action_parameters()
        {
            "~/funky/bar/widget".Route().ShouldMapTo<FunkyController>(x => x.Bar("something_else"));
        }

        [Test]
        public void should_be_able_to_test_routes_directly_from_a_string()
        {
            "~/funky/bar/widget".ShouldMapTo<FunkyController>( x => x.Bar( "widget" ) );
        }

        [Test]
        public void should_be_able_to_test_routes_with_member_expressions_being_used()
        {
            var widget = "widget";

            "~/funky/bar/widget".ShouldMapTo<FunkyController>( x => x.Bar( widget ) );
        }

		[Test]
		public void should_be_able_to_test_routes_with_member_expressions_being_used_but_ignore_null_complex_parameteres()
		{
			

			"~/funky/List".ShouldMapTo<FunkyController>(x => x.List(null));
		}

        [Test]
        public void should_be_able_to_ignore_requests()
        {
            "~/someimage.gif".ShouldBeIgnored();
        }

        [Test]
        public void should_be_able_to_ignore_requests_with_path_info()
        {
            "~/someimage.gif/with_stuff".ShouldBeIgnored();
        }

		[Test]
		public void should_be_able_to_match_non_string_action_parameters()
		{
			"~/funky/foo/1234".Route().ShouldMapTo<FunkyController>(x => x.Foo(1234));
		}

        [Test]
        public void assertion_exception_should_hide_the_test_helper_frames_in_the_call_stack()
        {
            IEnumerable<string> callstack=new string[0];
            try
            {
                "~/badroute that is not configures/foo/1234".Route().ShouldMapTo<FunkyController>(x => x.Foo(1234));
            }
            catch(Exception ex)
            {

                callstack = ex.StackTrace.Split(new string[] { Environment.NewLine },StringSplitOptions.None);
            }
            callstack.Count().ShouldEqual(1);
            
        }
        
        [Test]
        public void should_be_able_to_generate_url_from_named_route()
        {
            RouteTable.Routes.Clear();
            RouteTable.Routes.MapRoute(
                "namedRoute",
                "{controller}/{action}/{id}",
                new { controller = "Funky", Action = "Index", id = "" });

            OutBoundUrl.OfRouteNamed("namedRoute").ShouldMapToUrl("/");
        }

        [Test]
        public void should_be_able_to_generate_url_from_controller_action_where_action_is_default()
        {
            OutBoundUrl.Of<FunkyController>(x => x.Index()).ShouldMapToUrl("/");
        }

        [Test]
        public void should_be_able_to_generate_url_from_controller_action()
        {
            OutBoundUrl.Of<FunkyController>(x => x.New()).ShouldMapToUrl("/Funky/New");
        }

        [Test]
        public void should_be_able_to_generate_url_from_controller_action_with_parameter()
        {
            OutBoundUrl.Of<FunkyController>(x => x.Foo(1)).ShouldMapToUrl("/Funky/Foo/1");
        }
    }
}
