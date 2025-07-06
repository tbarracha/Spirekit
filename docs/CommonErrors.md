# Common Errors Related to Service Lifetime Marker Interfaces

### 1. **Missing Marker Interface on Service**

**Error Message:**

```
InvalidOperationException: Unable to resolve service for type 'YourNamespace.YourService' while attempting to activate 'SomeOtherClass'.
```

**Cause:**
Your service class does **not** implement one of the required marker interfaces (`ITransientService`, `IScopedService`, or `ISingletonService`),
so your DI auto-registration logic skips it, and the service cannot be resolved.

**Solution:**

* Add the appropriate marker interface to your service:

  * For transient lifetime: `public class YourService : ITransientService { ... }`
  * For scoped lifetime: `public class YourService : IScopedService { ... }`
  * For singleton lifetime: `public class YourService : ISingletonService { ... }`
* Rebuild your solution and restart the application.

---

### 2. **Ambiguous Lifetime (Multiple Marker Interfaces)**

**Error Message:**
No explicit error, but may cause unpredictable DI behavior.

**Cause:**
A service implements more than one marker interface, making its intended lifetime ambiguous.

**Solution:**

* Ensure every service implements **only one** marker interface (`ITransientService`, `IScopedService`, or `ISingletonService`).
