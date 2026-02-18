namespace DecidrV2;

public interface IEvolver<TState, in TEvent>
{
    TState CreateInitial();
    TState Evolve(TEvent evnt, TState state);
}
