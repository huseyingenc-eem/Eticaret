# RULES AND SPECS STANDARD

## Purpose
Team-wide standards for writing Business Rules (via `IBusinessRule<TRequest>`) and Specifications to keep semantics consistent, testable, and automatically enforced before handlers execute.

---

### 1) Rules Principles
- **Single Responsibility**: One rule → one concern (limit, ownership, uniqueness, default flag, state transition).
- **Fast `ShouldExecute`**: A cheap guard to skip unnecessary work.
- **Deterministic & Idempotent**: Same inputs → same outcome.
- **Priority Ordering**: Use `Priority` to guarantee stable sequencing (lower numbers run first, or follow project convention).
- **User-friendly errors**: Throw `BusinessException(message, userFriendlyMessage, errorCode)` and localize the friendly message.

#### Interface contract
```csharp
public interface IBusinessRule<in TRequest>
{
    int Priority { get; }
    bool ShouldExecute(TRequest request);
    Task ExecuteAsync(TRequest request, CancellationToken cancellationToken = default);
}
```

#### Example (Address Limit)
```csharp
public sealed class AddressLimitRule : IBusinessRule<CreateAddressCommand>
{
    private const int MAX = 10;
    private readonly IRepository<Address, Guid> _repo;

    public AddressLimitRule(IUnitOfWork uow) => _repo = uow.GetRepository<Address, Guid>();

    public int Priority => 20;
    public bool ShouldExecute(CreateAddressCommand cmd) => true;

    public async Task ExecuteAsync(CreateAddressCommand cmd, CancellationToken ct = default)
    {
        var spec = new CountByUserIdSpec(cmd.UserId);
        var count = await _repo.CountAsync(spec, ct);
        if (count >= MAX)
            throw new BusinessException(
                message: $"User {cmd.UserId} reached {MAX} addresses.",
                userFriendlyMessage: $"You can add at most {MAX} addresses.",
                errorCode: "ADDRESS_LIMIT_EXCEEDED");
    }

    private sealed class CountByUserIdSpec : Specification<Address>
    {
        public CountByUserIdSpec(string userId) : base(a => a.UserId == userId) {}
    }
}
```
### 2) Specifications Guidance
- Handler-level complex specs (joins/includes, sorting, paging) live under `Application/Features/<Feature>/Specifications/`.
- Rule-local simple specs (existence/uniqueness/count filters) are defined inside the rule class as private nested types for locality.
- Prefer composition (`AddInclude`, `AddOrderBy(Descending)`, `ApplyPaging`) and avoid leaking persistence details into Application code.

#### Examples

```csharp
// Handler-level (complex; reused by multiple handlers)
public sealed class DefaultAddresses : Specification<Address>
{
    public DefaultAddresses(string userId, bool? isShipping = null, bool? isBilling = null)
      : base(a => a.UserId == userId
               && (isShipping == null || a.IsDefaultShipping == isShipping)
               && (isBilling  == null || a.IsDefaultBilling  == isBilling))
    {
        AddOrderBy(a => a.AddressTitle);
    }
}

// Query optimized list
public sealed class UserAddressesOrdered : Specification<Address>
{
    public UserAddressesOrdered(string userId)
      : base(a => a.UserId == userId)
    {
        AddOrderByDescending(a => a.IsDefaultShipping);
        AddOrderByDescending(a => a.IsDefaultBilling);
        AddOrderBy(a => a.AddressTitle);
    }
}
```

### 3) Error & Telemetry
- Always set a stable `errorCode` (e.g., `ADDRESS_LIMIT_EXCEEDED`) and a localized `userFriendlyMessage`.
- Emit structured logs with category `RulesEngine.<RuleName>` and carry correlation/user ids from `IRequestInfoRequest`.

---

### 4) Registration & Execution
- Register rules in DI so the pre-handler pipeline can discover `IBusinessRule<TRequest>` implementations automatically for each request.
- Respect `Priority` ordering across rules. Example execution order:
  1. Ownership/Authorization (if any)
  2. Limits
  3. Uniqueness
  4. Default-flag housekeeping
- Handlers should not be aware of when rules run; they consume a request that is already “semantically valid”.

---

### 5) Testing Patterns
- Unit tests per rule with fake/in-memory repositories.
- Contract tests ensuring adding a new feature rule requires only wiring selectors/DI, not changes to the pipeline.
- Integration tests to verify rule execution ordering and that handlers are not called when a rule fails.

---

### 6) Naming & Structure
- **Rules**: `Application/Features/<Feature>/Rules/<MeaningfulRuleName>.cs`
- **Specs**:
  - **Handler-level**: `Application/Features/<Feature>/Specifications/*.cs`
  - **Rule-local**: private nested types within the rule
- **Error codes**: `DOMAIN_ACTION_REASON` pattern (e.g., `ADDRESS_CREATE_LIMIT_EXCEEDED`).

---

### How to Use This Standard
1. Before coding, identify feature inputs, business rules, and specs needed.
2. Create files following the `FEATURE_DEVELOPMENT_PLAYBOOK`.
3. For semantics, add Rules (this standard) and Specs (handler-level or rule-local).
4. Run tests and check the Final Checklist from the Playbook.