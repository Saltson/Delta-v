using Content.Shared.Actions;
using Content.Shared.Actions.ActionTypes;
using Content.Shared.StatusEffect;
using Content.Shared.Popups;
using Robust.Shared.Prototypes;
using Robust.Shared.Player;

namespace Content.Shared.Abilities.Psionics
{
    public sealed class MetapsionicPowerSystem : EntitySystem
    {
        [Dependency] private readonly IPrototypeManager _prototypeManager = default!;
        [Dependency] private readonly StatusEffectsSystem _statusEffects = default!;
        [Dependency] private readonly SharedActionsSystem _actions = default!;
        [Dependency] private readonly EntityLookupSystem _lookup = default!;
        [Dependency] private readonly SharedPopupSystem _popups = default!;


        public override void Initialize()
        {
            base.Initialize();
            SubscribeLocalEvent<MetapsionicPowerComponent, ComponentInit>(OnInit);
            SubscribeLocalEvent<MetapsionicPowerComponent, ComponentShutdown>(OnShutdown);
            SubscribeLocalEvent<MetapsionicPowerComponent, MetapsionicPowerActionEvent>(OnPowerUsed);
        }

        private void OnInit(EntityUid uid, MetapsionicPowerComponent component, ComponentInit args)
        {
            if (_prototypeManager.TryIndex<InstantActionPrototype>("MetapsionicPulse", out var metapsionicPulse))
                _actions.AddAction(uid, new InstantAction(metapsionicPulse), null);
        }

        private void OnShutdown(EntityUid uid, MetapsionicPowerComponent component, ComponentShutdown args)
        {
            if (_prototypeManager.TryIndex<InstantActionPrototype>("MetapsionicPulse", out var metapsionicPulse))
                _actions.RemoveAction(uid, new InstantAction(metapsionicPulse), null);
        }

        private void OnPowerUsed(EntityUid uid, MetapsionicPowerComponent component, MetapsionicPowerActionEvent args)
        {
            foreach (var entity in _lookup.GetEntitiesInRange(uid, component.Range))
            {
                if (HasComp<PsionicComponent>(entity) && entity != uid)
                {
                    _popups.PopupEntity(Loc.GetString("metapsionic-pulse-success"), uid, Filter.Entities(uid), PopupType.LargeCaution);
                    args.Handled = true;
                    return;
                }
            }
            _popups.PopupEntity(Loc.GetString("metapsionic-pulse-failure"), uid, Filter.Entities(uid), PopupType.Large);
            args.Handled = true;
        }
    }

    public sealed class MetapsionicPowerActionEvent : InstantActionEvent {}
}