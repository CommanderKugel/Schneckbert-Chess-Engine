
public static class SliderHelper
{

    public static ulong rookAttacksLoop (int index, ulong blockers)
    {
        ulong ret=0, one=1, next;
        int file = index % 8, rank = index / 8, r, f;

        for (r=rank+1; r<8; r++) {      // down
            next = one << (file + r*8);
            ret |= next;
            if ((blockers & next) != 0) break;
        }
        for (r=rank-1; r>=0; r--) {     // up
            next = one << (file + r*8);
            ret |= next;
            if ((blockers & next) != 0) break;
        }
        for (f=file+1; f<8; f++) {      // right
            next = one << (f + rank*8);
            ret |= next;
            if ((blockers & next) != 0) break;
        }
        for (f=file-1; f>=0; f--) {     // left
            next = one << (f + rank*8);
            ret |= next; 
            if ((blockers & next) != 0) break;
        }
        return ret;
    }

    
    public static ulong bishopAttacksLoop (int index, ulong blocker)
    {
        ulong ret=0, one=1, next;
        int file=index%8, rank=index/8, r, f;

        for (r=rank+1, f=file+1; r<8 && f<8; r++, f++) {
            next = one << f+r*8;
            ret |= next;
            if ((blocker & next) != 0) break;
        }
        for (r=rank+1, f=file-1; r<8 && f>=0; r++, f--) {
            next = one << f+r*8;
            ret |= next;
            if ((blocker & next) != 0) break;
        }
        for (r=rank-1, f=file+1; r>=0 && f<8; r--, f++) {
            next = one << f+r*8;
            ret |= next;
            if ((blocker & next) != 0) break;
        }
        for (r=rank-1, f=file-1; r>=0 && f>=0; r--, f--) {
            next = one << f+r*8;
            ret |= next;
            if ((blocker & next) != 0) break;
        }
        return ret;
    }

}