
using System.Numerics;

public class Magic
{
    
    public static ulong[][] rookAttacks   = new ulong[64][];
    public static ulong[][] bishopAttacks = new ulong[64][];

    public static readonly ulong[] rookMasks = { 282578800148862, 565157600297596, 1130315200595066, 2260630401190006, 4521260802379886, 9042521604759646, 18085043209519166, 36170086419038334, 282578800180736, 565157600328704, 1130315200625152, 2260630401218048, 4521260802403840, 9042521604775424, 18085043209518592, 36170086419037696, 282578808340736, 565157608292864, 1130315208328192, 2260630408398848, 4521260808540160, 9042521608822784, 18085043209388032, 36170086418907136, 282580897300736, 565159647117824, 1130317180306432, 2260632246683648, 4521262379438080, 9042522644946944, 18085043175964672, 36170086385483776, 283115671060736, 565681586307584, 1130822006735872, 2261102847592448, 4521664529305600, 9042787892731904, 18085034619584512, 36170077829103616, 420017753620736, 699298018886144, 1260057572672512, 2381576680245248, 4624614895390720, 9110691325681664, 18082844186263552, 36167887395782656, 35466950888980736, 34905104758997504, 34344362452452352, 33222877839362048, 30979908613181440, 26493970160820224, 17522093256097792, 35607136465616896, 9079539427579068672, 8935706818303361536, 8792156787827803136, 8505056726876686336, 7930856604974452736, 6782456361169985536, 4485655873561051136, 9115426935197958144, 0 };
    public static readonly ulong[] bishopMasks = { 18049651735527936, 70506452091904, 275415828992, 1075975168, 38021120, 8657588224, 2216338399232, 567382630219776, 9024825867763712, 18049651735527424, 70506452221952, 275449643008, 9733406720, 2216342585344, 567382630203392, 1134765260406784, 4512412933816832, 9024825867633664, 18049651768822272, 70515108615168, 2491752130560, 567383701868544, 1134765256220672, 2269530512441344, 2256206450263040, 4512412900526080, 9024834391117824, 18051867805491712, 637888545440768, 1135039602493440, 2269529440784384, 4539058881568768, 1128098963916800, 2256197927833600, 4514594912477184, 9592139778506752, 19184279556981248, 2339762086609920, 4538784537380864, 9077569074761728, 562958610993152, 1125917221986304, 2814792987328512, 5629586008178688, 11259172008099840, 22518341868716544, 9007336962655232, 18014673925310464, 2216338399232, 4432676798464, 11064376819712, 22137335185408, 44272556441600, 87995357200384, 35253226045952, 70506452091904, 567382630219776, 1134765260406784, 2832480465846272, 5667157807464448, 11333774449049600, 22526811443298304, 9024825867763712, 18049651735527936, 0 };

    public static readonly ulong[] rookMagics = { 9259400972386964096, 1733886131684507776, 738594738018469888, 18034791014731784, 72066390709830660, 72061993292464130, 1468174613476409349, 180145223127536896, 153333494704377888, 1152992148363173890, 54223515504607296, 3045614223657812480, 5075380042022913, 23082083035513520, 2269393073537040, 9597182558041277504, 288266110422224960, 2468043033263218688, 396598862539669632, 1656981471363592, 72131812159260672, 2323931625873735808, 1152923978583642370, 4613959809587611912, 2251868801646656, 288265596025978881, 2265201722276097, 36037594187040778, 299359354749984, 4689658986096526114, 578752685902153249, 882846265543622688, 8214605852517076992, 671115716582182912, 10376997314811134984, 22128845914624, 81117604492083233, 5859783450336199680, 5782667002602588230, 2674013353541664, 4574037093269504, 307371981324452392, 1161933102982103880, 9223389630115742724, 18176368971785, 162140581768758272, 9223389630125573152, 361554677411619073, 140738864155664, 37172444804423712, 4791909168495984674, 4612251167943360544, 22658770071027720, 612493948451555360, 9367566940598460420, 18446744071599820864, 74590938623672321, 11294528254362114, 1153071313357635653, 576608224368656409, 324823241426337802, 579979242159835137, 36591748055040258, 18446744072971387138, 0 };
    public static readonly ulong[] bishopMagics = { 72136793637523472, 11709394354324971586, 4613093463137714244, 1232579645711337472, 144749057615593984, 144265272503566336, 22588505662193922, 577096270829610016, 105554475368480, 72075324874100882, 1765588625617064448, 1158562000336326663, 1198469025104000, 2199291772996, 4399137065000, 10448366546115101696, 9241670127651667984, 9223654612421427328, 2814767081324552, 149894359705256072, 140790104068112, 8797244432412, 17729792901396, 149552409678864, 5206341524614545541, 4686021802168812672, 4665740914525077537, 853222113689856, 1407649962868800, 720611401375686656, 11620417509451644944, 289089094741395584, 4647996497789980808, 4648278041351717636, 288802191991507088, 35734667395584, 55240014472282624, 2886246627931066882, 9227880035747104000, 4613937955713646656, 2251902911775809, 864765912996135040, 7037733478334976, 4620693356467454976, 289426679234429444, 9008368561684690, 1752322553413632144, 2328783289105514514, 9224533671437244496, 36169552027255057, 3386565747376640, 1126449882341382, 9359112232255496, 18446744072644796928, 2332943806183407680, 2306003572606905344, 147075107758547104, 9007217516875776, 2342997707223733250, 1152921505681901056, 18446744071606108744, 18446744072724039760, 2342158813162899616, 10169795913981964, 0 };


    public static void initMagic ()
    {
        foreach (bool isRook in new bool[] { true, false })
        {
            ulong[] magics=isRook ? rookMagics : bishopMagics;
            int shift= isRook ? 12 : 9;

            for (int index=0; index<64; index++)
            {
                ulong magicNum=magics[index];
                tryFillAttacks(index, magicNum, shift, isRook);
            }
        }
    }

    public static bool tryFillAttacks (int index, ulong magicNum, int shift, bool isRook)
    {
        ulong standardMask = isRook ? rookMasks[index] : bishopMasks[index];
        ulong[] blockerOcc = generateAllBlockerConfigs(standardMask);

        // reset rookAttacks array
        ulong[][] attacks = isRook ? rookAttacks : bishopAttacks;
        attacks[index]=new ulong[1ul<<shift];
        for (int i=0; i<(1<<shift); i++) attacks[index][i]=0;

        foreach (ulong blocker in blockerOcc)
        {
            ulong key = (magicNum * blocker) >> (64-shift);
            ulong attack = isRook ? SliderHelper.rookAttacksLoop(index, blocker) : SliderHelper.bishopAttacksLoop(index, blocker);
            
            if (attacks[index][key] == 0) attacks[index][key]=attack;
            else if (attacks[index][key] != attack) return false;
        }
        return true;
    }


    public static ulong[] generateAllBlockerConfigs (ulong mask)
    {
        ulong copy = mask;
        int squareAmount = BitOperations.PopCount(mask);
        int blockerAmt = 1<<squareAmount;
        ulong[] allBlockerConfigs = new ulong[blockerAmt];
        
        // mapping index to relevant square
        int[] squareIndizes = new int[squareAmount];                
        for (int i=0; copy!=0; i++)
        {
            int nextIndex = Helper.popLSB(ref copy);
            squareIndizes[i] = nextIndex;
        } 
        // loop over all Blocker configs
        for (int i=0; i<blockerAmt; i++) 
        {
            ulong blockerMask = 0;
            for (int bitIndex=0; bitIndex<squareAmount; bitIndex++) 
            {
                if ((i & (1 << bitIndex)) != 0) 
                {
                blockerMask |= 1ul << squareIndizes[bitIndex];
                }
            }
            allBlockerConfigs[i] = blockerMask;
        }   
        return allBlockerConfigs;
    }
}