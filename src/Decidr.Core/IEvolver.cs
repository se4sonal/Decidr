namespace Decidr;

public interface IEvolver<TState, in TEvent>
{
    TState CreateInitial();
    TState Evolve(TState current, TEvent evnt);
}
