using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * This class contains helper functions which are reused in other classes
 */

public class Helpers : MonoBehaviour {
    public static string RandomString(int length = 5, bool numeric = false) {
        string glyphs;
        if (numeric) {
            glyphs = "0123456789"; //add the characters you want
        } else {
            glyphs = "abcdefghijklmnopqrstuvwxyz"; //add the characters you want
        }

        if (length > glyphs.Length) length = glyphs.Length;

        var myString = "";
        for (int i = 0; i < length; i++) {
            myString += glyphs[Random.Range(0, glyphs.Length)];
        }

        return myString;
    }
}