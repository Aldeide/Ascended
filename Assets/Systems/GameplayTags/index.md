# Gameplay Tags

Gameplay tags are hierarchical strings that can be applied to different elements such as abilities, effects, gameplay
cues or even ability systems as a whole.

They can be used to define the various interactions that are or aren't possible. For example:

A gameplay tag Status.Immune.Stun can be granted to a player through its ability system. An ability that applies a stun
effect should specify Status.Immune.Stun in its TargetBlockedTags.

That is not the only application of gameplay tags. They can also be used to uniquely identify a gameplay cue and easily
pass the tag over the network to tell clients that a specific cue needs to be executed once, started or stopped.