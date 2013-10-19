TinyCqrs
========

This is a lightweight prototype library that I am planning on using in order to decouple some legacy projects.  My particular challenge is to take static database calls, and replace them with a testable approach to data access.  My first attempt to do this was through converting the static methods to services, and injecting mock services when testing.

However, I have so many commands (to update data) and queries (to read data) that it felt better (at the time) to abstract along those lines.

I am still using services for dependencies that are lower down in the stack, but trying to expose the commands and queries directly.

This is not intended for distributed applications, my particular projects interact directly with a sql db.
