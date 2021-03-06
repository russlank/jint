// Copyright 2009 the Sputnik authors.  All rights reserved.
/**
 * The Date.prototype.getUTCMinutes property "length" has { ReadOnly, DontDelete, DontEnum } attributes
 *
 * @path ch15/15.9/15.9.5/15.9.5.21/S15.9.5.21_A3_T3.js
 * @description Checking DontEnum attribute
 */

if (Date.prototype.getUTCMinutes.propertyIsEnumerable('length')) {
  $ERROR('#1: The Date.prototype.getUTCMinutes.length property has the attribute DontEnum');
}

for(x in Date.prototype.getUTCMinutes) {
  if(x === "length") {
    $ERROR('#2: The Date.prototype.getUTCMinutes.length has the attribute DontEnum');
  }
}


