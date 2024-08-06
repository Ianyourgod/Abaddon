using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stats {    
    public enum Type {
        Wisdom,
        Strength,
        Dexterity,
        Constitution,
    }

    public Action<int[]> onChangeStats;
    public Action<int> onChangeConstitution;
    public Action<int> onChangeDexterity;
    public Action<int> onChangeStrength;
    public Action<int> onChangeWisdom;

    private int _constitution = 9;
    private int _dexterity = 9;
    private int _strength = 9;
    private int _wisdom = 9;

    public int constitution { 
        get => _constitution; 
        set {
            _constitution = value;
            onChangeStats?.Invoke(ToArray());
            onChangeConstitution?.Invoke(_constitution);
        }
    }

    public int dexterity { 
        get => _dexterity; 
        set {
            _dexterity = value;
            onChangeStats?.Invoke(ToArray());
            onChangeDexterity?.Invoke(_dexterity);
        }
    }

    public int wisdom { 
        get => _wisdom; 
        set {
            _wisdom = value;
            onChangeStats?.Invoke(ToArray());
            onChangeWisdom?.Invoke(_wisdom);
        }
    }

    public int strength {
        get => _strength; 
        set {
            _strength = value;
            onChangeStats?.Invoke(ToArray());
            onChangeStrength?.Invoke(_strength);
        }
    }

    public int GetMaxHealth() {
        return constitution * 2;
    }

    public int GetAttackDamage() {
        return 2 + ((_strength - 10) / 2);
    }

    public float DexterityToPercent() {
        return _dexterity / 20f;
    }

    public int[] ToArray() {
        return new int[] {
            _dexterity,
            _constitution,
            _strength,
            _wisdom
        };
    }

    public Stats() {
        _constitution = 0;
        _dexterity = 0;
        _strength = 0;
        _wisdom = 0;
    }

    public Stats(int statVariance, int startlevel = 9) {
        _constitution = startlevel + UnityEngine.Random.Range(0, statVariance);
        _dexterity = startlevel + UnityEngine.Random.Range(0, statVariance);
        _strength = startlevel + UnityEngine.Random.Range(0, statVariance);
        _wisdom = startlevel + UnityEngine.Random.Range(0, statVariance);
    }

    public Stats(int constitution, int dexterity, int strength, int wisdom, int statVariance = 0) {
        _constitution = constitution + UnityEngine.Random.Range(0, statVariance);
        _dexterity = dexterity + UnityEngine.Random.Range(0, statVariance);
        _strength = strength + UnityEngine.Random.Range(0, statVariance);
        _wisdom = wisdom + UnityEngine.Random.Range(0, statVariance);
    }

    public int[] AddToStats(int constitution, int dexterity, int strength, int wisdom) {
        _constitution += constitution;
        _dexterity += dexterity;
        _strength += strength;
        _wisdom += wisdom;
        return new int[] {
            dexterity,
            constitution,
            strength,
            wisdom
        };
    }

    public void SubtractFromStats(int constitution, int dexterity, int strength, int wisdom) {
        _constitution -= constitution;
        _dexterity -= dexterity;
        _strength -= strength;
        _wisdom -= wisdom;
    }
}
