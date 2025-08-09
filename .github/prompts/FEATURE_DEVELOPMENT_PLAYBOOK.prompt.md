# FEATURE DEVELOPMENT PLAYBOOK

## Purpose
A precise, repeatable playbook for adding a new feature to a .NET solution that follows Clean Architecture + CQRS, integrates pipeline behaviors (Authorization, Transactional, Caching, RequestInfo), and leverages a Rules Engine with Specifications.

---

### 1) Role & Non-Negotiables
You are a developer experienced in .NET, Clean Architecture, and CQRS. Your primary responsibility is to implement the feature without violating dependency rules or layer responsibilities.

#### Conventions
- No non-ASCII letters in identifiers. Use `PascalCase` (for types/namespaces) and `camelCase` (for locals/params); `_field` for private fields.
- Controllers must be thin: no business logic, no repository access.
- Syntactic validation → FluentValidation; Semantic/business validation → Rules Engine.

---

### 2) Architecture Overview (Onion)
- **Domain**: Entities, Value Objects, domain logic. No dependencies outward.
- **Application**: Commands/Queries, DTOs, interfaces, validators, business rules. Depends only on Domain.
- **Persistence**: EF Core implementations of Application interfaces. Depends on Application.
- **Infrastructure**: External services (email, cache, files). Depends on Application.
- **Presentation (API)**: Accepts requests, wires DI, calls MediatR. No business rules.

---

### 3) Define the Feature
Write 1–2 sentences that clearly state the feature goal.

> **Example**: “Allow a user to create a new address and optionally mark it as default for shipping or billing.”

#### Business Rules (examples)
- Title required, min length 3.
- Per-user limits apply (e.g., max 10 addresses).
- Only one default shipping/billing address per user.
- Domain events may be raised after creation (optional).
---

### 4) Files to Create (Minimal Set)

#### 4.1 `Application/Features/<Feature>/Commands/Create`
- **`Create<Feature>Command.cs`**
  - Implements `IRequest<ResponseDto>`. Implement `ITransactionalRequest`, `ICacheRemoverRequest`, `IRequestInfoRequest` if needed.
  - Optionally apply `[DefaultRoles("Admin","User")]`.
  - Include cache keys/group keys if you need cache invalidation.
- **`Create<Feature>CommandValidator.cs`**
  - Inherits from `AbstractValidator<Create<Feature>Command>`. Covers required/format/length/range checks.
- **`Create<Feature>CommandHandler.cs`**
  - Implements `IRequestHandler<Create<Feature>Command, ResponseDto>`.
  - Inject `IMapper`, repositories, and do not replicate business rules here—assume Rules Engine ran pre-handler.
  - Map → save → (raise event) → map to `ResponseDto`.
  > **Tip**: If orchestration gets complex, create a local `BusinessRules` helper class only for composition; discrete checks belong to the Rules Engine.

#### 4.2 `Application/Features/<Feature>/Rules`
- One or more `IBusinessRule<Create<Feature>Command>` implementing `ShouldExecute`, `ExecuteAsync`, and `Priority`.
- Throw `BusinessException` with `message`, `userFriendlyMessage`, and `errorCode` on violations.

#### 4.3 `Application/Features/<Feature>/Specifications`
- Handler-oriented complex specs (includes, ordering, paging).
- Keep small, rule-local specs inside rule classes.

#### 4.4 `Domain/Entities`
- Update/add POCO entities as needed, with no external dependencies.

#### 4.5 Persistence & Presentation
- Usually minimal/no change if using generic repositories.
- In the Controller, add an action → map request → `_mediator.Send(command)` → return result.

---

### 5) Behaviors Integration (When to Use)
- **Authorization**: Annotate the command with `[DefaultRoles(...)]`.
- **Transactional**: Commands that mutate state should implement `ITransactionalRequest`.
- **Caching**: Queries use `ICacheRequest`; write commands that invalidate related entries use `ICacheRemoverRequest`.
- **RequestInfo**: Use `IRequestInfoRequest` to carry `UserId`, correlation id, etc.

---

### 6) Example Skeleton (Create Address)

```csharp
[DefaultRoles("Admin","User")]
public sealed class CreateAddressCommand
  : IRequest<CreateAddressResponseDto>,
    ITransactionalRequest,
    ICacheRemoverRequest,
    IRequestInfoRequest
{
    [JsonIgnore]
    public string UserId { get; set; } = string.Empty;
    public string AddressTitle { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public bool IsDefaultShipping { get; set; }
    public bool IsDefaultBilling  { get; set; }

    // Caching
    public string? CacheKey => $"user-addresses_{UserId}";
    public bool BypassCache => false;
    public string? CacheGroupKey => null;
}

public sealed class CreateAddressCommandValidator
  : AbstractValidator<CreateAddressCommand>
{
    public CreateAddressCommandValidator()
    {
        RuleFor(x => x.AddressTitle).NotEmpty().MinimumLength(3);
        RuleFor(x => x.City).NotEmpty();
    }
}

public sealed class CreateAddressCommandHandler
  : IRequestHandler<CreateAddressCommand, CreateAddressResponseDto>
{
    private readonly IRepository<Address, Guid> _repo;
    private readonly IMapper _mapper;

    public CreateAddressCommandHandler(IUnitOfWork uow, IMapper mapper)
    {
        _repo = uow.GetRepository<Address, Guid>();
        _mapper = mapper;
    }

    public async Task<CreateAddressResponseDto> Handle(
        CreateAddressCommand request, CancellationToken ct)
    {
        // Assume semantic rules already executed by Rules Engine.
        var entity = _mapper.Map<Address>(request);
        await _repo.AddAsync(entity, ct);

        var dto = _mapper.Map<CreateAddressResponseDto>(entity);
        dto.Message = "Address created successfully.";
        return dto;
    }
}
```

### 7) Testing
- **Unit**: Test validators, rules, and handlers (mock repositories/services).
- **Integration**: Test the endpoint and the full pipeline (auth, transaction, caching).
- Assert cache invalidation on write paths and role rejections where applicable.

---

### 8) Final Checklist
- [✅] Dependencies flow correctly (Application does not depend on Persistence/Infrastructure).
- [✅] No business logic inside controllers.
- [✅] FluentValidation handles shape; Rules Engine handles semantics.
- [✅] Behaviors (Authorization/Transactional/Caching/RequestInfo) are attached appropriately.
- [✅] DTOs do not leak domain internals; mapping is verified.
