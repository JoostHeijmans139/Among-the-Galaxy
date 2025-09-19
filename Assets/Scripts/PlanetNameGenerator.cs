using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetNameGenerator : MonoBehaviour
{
    // Final planet name
    public string planetName = "planet_name";

    // Amount of letterCombos used
    public int comboAmount = 3;
    
    public void generatePlanetName()
    {
        // All two letter combinations with the letters a, e, i, o and u
        string[] letterCombos =
        {
            "aa","ab","ac","ad","ae","af","ag","ah","ai","aj","ak","al","am","an","ao","ap","aq","ar","as","at","au","av","aw","ax","ay","az",
            "ba","be","bi","bo","bu",
            "ca","ce","ci","co","cu",
            "da","de","di","do","du",
            "ea","eb","ec","ed","ee","ef","eg","eh","ei","ej","ek","el","em","en","eo","ep","eq","er","es","et","eu","ev","ew","ex","ey","ez",
            "fa","fe","fi","fo","fu",
            "ga","ge","gi","go","gu",
            "ha","he","hi","ho","hu",
            "ia","ib","ic","id","ie","if","ig","ih","ii","ij","ik","il","im","in","io","ip","iq","ir","is","it","iu","iv","iw","ix","iy","iz",
            "ja","je","ji","jo","ju",
            "ka","ke","ki","ko","ku",
            "la","le","li","lo","lu",
            "ma","me","mi","mo","mu",
            "na","ne","ni","no","nu",
            "oa","ob","oc","od","oe","of","og","oh","oi","oj","ok","ol","om","on","oo","op","oq","or","os","ot","ou","ov","ow","ox","oy","oz",
            "pa","pe","pi","po","pu",
            "qa","qe","qi","qo","qu",
            "ra","re","ri","ro","ru",
            "sa","se","si","so","su",
            "ta","te","ti","to","tu",
            "ua","ub","uc","ud","ue","uf","ug","uh","ui","uj","uk","ul","um","un","uo","up","uq","ur","us","ut","uu","uv","uw","ux","uy","uz",
            "va","ve","vi","vo","vu",
            "wa","we","wi","wo","wu",
            "xa","xe","xi","xo","xu",
            "ya","ye","yi","yo","yu",
            "za","ze","zi","zo","zu"
        };

        // Make random name
        planetName = "";


        for (int i = 0; i < comboAmount; i++)
        {
            int randomLetterComboNumber = Random.Range(0, letterCombos.Length + 1);

            planetName = planetName + $"{letterCombos[randomLetterComboNumber]}";
        }

        // Give planetName first letter uppercase
        planetName.ToUpper();

        //DEBUG show planet name
        Debug.Log(planetName);
    }
}
