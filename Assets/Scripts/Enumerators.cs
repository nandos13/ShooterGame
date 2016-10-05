
/* DESCRIPTION:
 * No classes declared in this file. Public Enumerators go here.
 */

// Used to decide whether collision actions should ignore colliders in a list, or all but those in the list.
public enum COLLISION_MODE { HitSelected, IgnoreSelected }

// Type of weapon, used in weapon script
public enum WEAPON_TYPE { Bullet, Launcher, Pulse, Beam }

// Behaviour state used by turrets
public enum TurretBehaviourState { PreparingFire, Firing, Searching }