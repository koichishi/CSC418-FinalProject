// Inputs:
//   theta  amount x which to rotate (in radians)
// Return a 4x4 matrix that rotates a given 3D point/vector about the x-axis by
// the given amount.
mat4 rotate_about_x(float theta)
{
  return mat4(
  1.0, 0, 0, 0,
  0, cos(theta), sin(theta), 0,
  0,-sin(theta), cos(theta), 0,
  0, 0, 0, 1.0);
}
