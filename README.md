# NDjangoSignCookie

Django Sign Cookie support for asp.net.

So that Django app & asp.net website could share the same signed cookie.

The cookie signed by Django, could be read/unsigned by asp.net, or vise versa.

# How to use?

import `NDjangoSignCookie` namespace:

    using NDjangoSignCookie;

And:

* `SetSignedCookie` extention method will be added to `HttpResponse` object
* `GetSignedCookie` extention method will be added to `HttpRequest` object

# Todo

* Add max-age support when reading cookie.
