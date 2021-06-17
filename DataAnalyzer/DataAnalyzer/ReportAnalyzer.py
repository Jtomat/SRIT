from numba import cuda 
import docx
import re
import math
import natasha
from natasha import (
    Segmenter,
    PER,
    ORG,
    LOC,
    NewsEmbedding,
    NewsMorphTagger,
    NewsSyntaxParser,
    NamesExtractor,
    Doc,
    MorphVocab,
    NewsNERTagger
)
import ctypes
import math
import itertools

morph_vocab = MorphVocab()
segmenter = Segmenter()
names_extractor = NamesExtractor(morph_vocab)

emb = NewsEmbedding()
morph_tagger = NewsMorphTagger(emb)
syntax_parser = NewsSyntaxParser(emb)


def getText(filename):
    doc = docx.Document(filename)
    fullText = []
    for para in doc.paragraphs:
        fullText.append(para.text.replace(' г.',' год'))
    return fullText

#regEx=r"([А-яЁё\s\d.-—,]*)Документ №[1-9\*]+([А-яЁё\s\d.-—,]*)"
regEx=r"([ \n\r\xa0]+)"
def getDocArray(text):
    resultEx=re.split(regEx,text)
    return text
def getBoolW(word,num,endNum,cn):
    if word not in selectedWords:
        selectedWords.append(word)
    if word not in allWords[cn].keys():
       supMemory=[]
       empty1=[]
       empty2=[]
       for i in range(0,endNum):
           supMemory.append(0)
           empty1.append(0)
           empty2.append(0)
       if(num==1):
           d=0
       supMemory[num]=1
       empty2[num]=cn
       allWords[cn][word]={"Bool":supMemory,
                        "TF":empty1}
    else:
        if allWords[cn][word]["Bool"][num]==0:
            allWords[cn][word]["Bool"][num]=1

def getTFW(word,num,cn):
    allWords[cn][word]["TF"][num]+=1

def getLems(doc):
    res=[]
    insert_ind=[]
    i=0
    for token in doc.tokens:        
        token.lemmatize(morph_vocab)
        if(token.lemma=="который"):
            l=0
        if token.pos not in ['PUNCT','ADP','NUM','AUX','CCONJ','SCONJ','PRON']: #,'PROPN'
           if(len(token.lemma)>2):
            res.append(token.lemma)
        else:
            insert_ind.append(i)
        i+=1
    i=0
    return res

def getW(text,ind,endNum,cn):
    doc = Doc(text)
    doc.segment(segmenter)
    doc.tag_morph(morph_tagger)
    doc.parse_syntax(syntax_parser)
    ner_tagger = NewsNERTagger(emb)
    doc.tag_ner(ner_tagger)
    list=getLems(doc)
    for word in list:
        getBoolW(word,ind,endNum,cn)
        getTFW(word,ind,cn)
        #getTFIDFW(word,ind,len(list))
def Sort(sub_li): 
    sub_li.sort(key = lambda x: x[1]) 
    return sub_li 



def getLines():
    res =[]
    for i,abz in enumerate(abzs):
        line = ""
        if(abz==""):
            continue
        for st in re.split(r'\.|\?',abz):
            need = True
            for c in chr:
                if c in st:
                    need = False
                    break
            if (need) and st!='' and st!=' ' and st!=None:
                line+=" "+st
            
                doc = Doc(st)
                doc.segment(segmenter)
                doc.tag_morph(morph_tagger)
                doc.parse_syntax(syntax_parser)
                ner_tagger = NewsNERTagger(emb)
                doc.tag_ner(ner_tagger)
                list=getLems(doc)
                L =0
                N=len(list)
                for ws in list:
                    if allWords[ws]["Type"] == "ГОС" or allWords[ws]["Type"] == "ВОС":
                        L+=1;
                if(N!=0):
                    res.append([st,L/N])
    se = Sort(res)
    return se

def keyGet(line):
    doc = Doc(line)
    doc.segment(segmenter)
    doc.tag_morph(morph_tagger)
    doc.parse_syntax(syntax_parser)
    ner_tagger = NewsNERTagger(emb)
    doc.tag_ner(ner_tagger)
    list=getLems(doc)
    res = []
    now_c=-1
    exp = []
    for w in list:
        now_c+=1
        if allWords[w]["Type"]=="ГОС" or allWords[w]["Type"]=="ВОС":
            if allWords[(list[now_c-1])]["Type"]=="ГОС" or allWords[(list[now_c-1])]["Type"]=="ВОС":
                if now_c < len(list)-1 and (allWords[(list[now_c+1])]["Type"]=="ГОС" or allWords[(list[now_c+1])]["Type"]=="ВОС"):
                    res.append(f"{list[now_c-1]} {list[now_c]} {list[now_c+1]}")
                else:
                    ned = True
                    for wd in res:
                        if (f"{list[now_c-1]} {list[now_c]}") in wd or (f" {list[now_c]} {list[now_c-1]}") in wd:
                            fs = [list[now_c-1],list[now_c]]
                            newd = wd.split(' ')
                            fd = ""
                            for  sf in fs:
                                if sf not in newd:
                                    newd.append(sf)
                            for sc in newd:
                                fd+=sc+" "
                            exp.append(fd)
                            ned = False
                    if(ned):
                        res.append(f"{list[now_c-1]} {list[now_c]}")
    return res

global selectedWords
selectedWords=[]
global allWords
allWords={}

def Find_KeyWords(text):
    global selectedWords
    selectedWords=[]
    global allWords
    allWords={}
    #print("Введите номер от 1 до 5:")
    abzs = text#List<strring> (getText(f"текст {input()}.docx"))
    text_counter =0;
    chr = ["\"","«","»"]
    for i,abz in enumerate(abzs):
        line = ""
        if(abz==""):
            continue
        for st in re.split(r'\.|\?',abz):
            need = True
            for c in chr:
                if c in st:
                    need = False
                    break
            if(need):
                line+=" "+st

        doc = Doc(line.strip())
        doc.segment(segmenter)
        doc.tag_morph(morph_tagger)
        doc.parse_syntax(syntax_parser)
        ner_tagger = NewsNERTagger(emb)
        doc.tag_ner(ner_tagger)
        lister=getLems(doc)
        for word in lister:
            text_counter+=1
            if(word in allWords.keys()):
                if i not in allWords[word]["abz_count"]:
                   allWords[word]["abz_count"].append(i)
                allWords[word]["text_count"]+=1
            else: 
                allWords[word]={"abz_count":[i],
                            "text_count":1,
                            "Type":None,
                            "K":0.0}
    KGOS = (9/(text_counter*len(abzs)))
    KVOS = (1+len(abzs)/4)*(1+len(abzs)/4)/(text_counter*len(abzs))
    for word in allWords.keys():
        allWords[word]["K"] =  allWords[word]["text_count"] *  len(allWords[word]["abz_count"])/(text_counter*len(abzs))
        if allWords[word]["K"] >= KGOS and allWords[word]["K"]<1:
            allWords[word]["Type"]="ГОС"
        elif allWords[word]["K"]>=KVOS and allWords[word]["K"]<KGOS:
            allWords[word]["Type"]="ВОС"
        else:
            allWords[word]["Type"]="Н"
    #if allWords[word]["Type"]=="ГОС" or  allWords[word]["Type"]=="ВОС":
    #print(F"{word:25} {allWords[word]['Type']:5}",allWords[word])

    key_wd = []
    for i,abz in enumerate(abzs):
        line = ""
        if(abz==""):
            continue
        for st in re.split(r'\.|\?',abz):
            need = True
            for c in chr:
                if c in st:
                    need = False
                    break
            if(need):
                line+=" "+st
        if line.strip()!='':
            cs = keyGet(line)
            if len(cs)>0:
                for ad in cs:
                    nd = True
                    for w in key_wd:
                        if ad in w:
                            nd = False
                    if nd:
                        key_wd.append(ad)
    #print("Keywords:")
    end_w = []
    not_so =[]
    for line in key_wd:
        not_so.append(sorted(line.split(), key=str.lower));
    not_so.sort()
    end_w = list(not_so for not_so,_ in itertools.groupby(not_so))
    end_w=list(tuple(i) for i in end_w)
    final_charge=set()
    for i,line in enumerate(end_w):
        for j,next in enumerate(end_w):
            if(i==j):
                continue
            if(set(line)<=set(next)):
                final_charge.add(line)
            else: 
                if(set(line)>=set(next) or set(line)==set(next)):
                    final_charge.add(next)
    end_w = list(set(end_w)-final_charge)
        #print('',ws)
    #for ds in end_w:
    #    print(ds)
    #an=""
    #vd=getLines()[:3]
    #for lj in vd:
    #    an+=lj[0]+"."
        #print(lj)
    #print("Аннотация:")
    #print('',an)
    #input()
    return end_w
#Find_KeyWords('')

def Check_Two_Records(current,mainRecord):
    bools = [[],[]]
    for KeyData in mainRecord:
        if KeyData in current.keys():
            if type(mainRecord[KeyData]) !='list': 
                if mainRecord[KeyData] == current[KeyData]:
                    bools[0].append(1)
                    bools[1].append(1)
                else:
                    if KeyData!='Solution':
                        bools[0].append(0)
                        bools[1].append(0)
            else:
                for keyWord in mainRecord[KeyData]:
                    if key in current.keys():
                        bools[0].append(1)
                        bools[1].append(1)
                    else:
                        bools[0].append(0)
                        bools[1].append(0)
    sum = 0 # верхняя часть
    s_d_1=0 # нижняя левая часть
    s_d_2=0 # нижняя правая часть
    for i in  range(0,len(bools[0])):
        sum+=bools[0][i]*bools[1][i]
        s_d_1+=bools[0][i]*bools[0][i]
        s_d_2+=bools[1][i]*bools[1][i]
    cos = sum/(s_d_1+s_d_2)    
    return cos

def Find_Dublicate(this,all):
    cos_bool_all = {}
    for rec in all:
        cos_bool_all[rec] = Check_Two_Records(this,all)
    max=-float('inf')
    id=0
    for c in cos_bool_all:
        if max<cos_bool_all[c]:
            max=cos_bool_all[c]
            id=c
    return id