using Nancy;
using System.Collections.Generic;
using System;

namespace Hospital
{
  public class HomeModule : NancyModule
  {
    public HomeModule()
    {
      Get["/"] = _ => "hello world";
    }
  }
}
