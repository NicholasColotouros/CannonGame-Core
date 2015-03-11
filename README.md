This is the core of a cannon game for use in Unity3D. The physics are implemented seperately of Unity's own physics.
One cannon fires cannonballs which use Euler integration while the other fires a dog consistent with Verlet integration.

Both cannons have wind resistance and are affected by the wind (represented by an arrow on screen).

Controls:
+------------+---------------+-----------------+
|   ACTION   | Verlet Cannon |  Normal Cannon  |
+------------+---------------+-----------------+
| Angle++    | W             | up arrow key    |
| Angle--    | S             | down arrow key  |
| Strength++ | D             | right arrow key |
| Strength-- | A             | left arrow key  |
| Fire       | tab	         | space		   |
+------------+---------------+-----------------+