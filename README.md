# NDjangoSignCookie

Django Sign Cookie support for asp.net.

So that Django app & asp.net website could share the same signed cookie.

The cookie signed by Django, could be read/unsigned by asp.net, or vise versa.

# How to use?

To install NDjangoSignCookie, run the following command in the Package Manager Console:

	PM> Install-Package NDjangoSignCookie

Add `SECRET_KEY` in web.config AppSettings, like

```
<configuration>
  <appSettings>
    <add key="SECRET_KEY" value ="My secret key may shared with Django app"/>
  </appSettings>
</configuration>
```

import `NDjangoSignCookie` namespace:

    using NDjangoSignCookie;

then 

* `SetSignedCookie` extention method will be added to `HttpResponse` object
* `GetSignedCookie` extention method will be added to `HttpRequest` object

Support both `System.Web` and `System.Web.MVC` request / response object.

# Todo

* Add max-age support when reading cookie.
