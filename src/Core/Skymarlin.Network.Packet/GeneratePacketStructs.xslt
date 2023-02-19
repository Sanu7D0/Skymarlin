<?xml version="1.0" encoding="utf-8" ?>
<xsl:stylesheet version="3.0"
                xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns:xs="http://www.w3.org/2001/XMLSchema" 
                xmlns:sky="urn:Skymarlin"
                exclude-result-prefixes="#all">
    <xsl:param name="namespace" />
    <xsl:output method="text" indent="yes" />
    <xsl:include href="Common.xslt" />
    
    <xsl:template match="sky:Packets">
        <xsl:text>using System;

namespace </xsl:text>
        <xsl:value-of select="$namespace" />
        <xsl:text>;</xsl:text>
        <xsl:apply-templates />
    </xsl:template>
    
    <xsl:template match="sky:Packet">
        <xsl:value-of select="$newline" />
        <xsl:text>public readonly struct </xsl:text>
        <xsl:apply-templates select="sky:Name" />
        <xsl:value-of select="$newline" />
        <xsl:text>{</xsl:text>
        <xsl:value-of select="$newline" />
        
        <xsl:text>    private readonly Memory&lt;byte&gt; _data;</xsl:text>
        <xsl:value-of select="$newline" />
        <xsl:value-of select="$newline" />
        
        <xsl:call-template name="constructor">
            <xsl:with-param name="packet" select="." />
        </xsl:call-template>
        
        <xsl:apply-templates select="sky:HeaderType" />
        <xsl:apply-templates select="sky:Code" />
        
        <xsl:text>    public </xsl:text>
        <xsl:value-of select="sky:HeaderType" />
        <xsl:text> Header => new (_data);</xsl:text>
        <xsl:value-of select="$newline" />
        
        <xsl:apply-templates select="sky:Fields" />
        
        <xsl:call-template name="implicitConversions" />
        
        <xsl:call-template name="lengthCalculator">
            <xsl:with-param name="struct" select="." />
        </xsl:call-template>
        
        <xsl:text>}</xsl:text>
        <xsl:value-of select="$newline" />
    </xsl:template>
    
    <xsl:template match="sky:HeaderType">
        <xsl:text>    public static byte HeaderType => 0x</xsl:text>
        <xsl:value-of select="substring(., 1, 2)" />
        <xsl:text>;</xsl:text>
        <xsl:value-of select="$newline" />
    </xsl:template>
    
    <xsl:template match="sky:Code">
        <xsl:text>    public static byte Code => 0x</xsl:text>
        <xsl:value-of select="." />
        <xsl:text>;</xsl:text>
        <xsl:value-of select="$newline" />
    </xsl:template>
    
    <xsl:template name="constructor">
        <xsl:param name="packet" as="sky:Packet" />
        <xsl:text>    public </xsl:text>
        <xsl:apply-templates select="sky:Name" />
        <xsl:text>(Memory&lt;byte&gt; data)
    : this(data, true)</xsl:text>
        <xsl:value-of select="$newline" />
        <xsl:text>    {</xsl:text>
        <xsl:value-of select="$newline" />
        <xsl:text>    }</xsl:text>
        <xsl:value-of select="$newline" />
        <xsl:value-of select="$newline" />
        
        <xsl:text>    private </xsl:text>
        <xsl:apply-templates select="sky:Name" />
        <xsl:text>(Memory&lt;byte&gt; data, bool initialize)</xsl:text>
        <xsl:value-of select="$newline" />
        <xsl:text>    {</xsl:text>
        <xsl:value-of select="$newline" />
        <xsl:text>        _data = data; </xsl:text>
        <xsl:value-of select="$newline" />
        <xsl:text>        if (initialize)
        {
            var header = Header;
            header.Type = HeaderType;
            header.Code = Code;
            header.Length = data.Length;
        }</xsl:text>
        <xsl:value-of select="$newline" />
        <xsl:text>    }</xsl:text>
        <xsl:value-of select="$newline" />
        <xsl:value-of select="$newline" />
    </xsl:template>
    
    <xsl:template name="lengthDataType">
        <xsl:param name="HeaderType" />
        <xsl:variable name="isByte" select="$HeaderType mod 2 = 1" />
        <xsl:choose>
            <xsl:when test="$isByte = 1">(byte)</xsl:when>
            <xsl:otherwise>(ushort)</xsl:otherwise>
        </xsl:choose>
    </xsl:template>
    
    <xsl:template name="lengthCalculator">
        <xsl:param name="struct" />
    </xsl:template>
    
    <xsl:template name="implicitConversions">
        <xsl:text>    public static implicit operator </xsl:text>
        <xsl:apply-templates select="sky:Name" />
        <xsl:text>(Memory&lt;byte&gt; packet) => new (packet, false); </xsl:text>
        <xsl:value-of select="$newline" />
        <xsl:value-of select="$newline" />
        
        <xsl:text>    public static implicit operator Memory&lt;byte&gt;(</xsl:text>
        <xsl:apply-templates select="sky:Name" />
        <xsl:text> packet) => packet._data;</xsl:text>
        <xsl:value-of select="$newline" />
    </xsl:template>
    
    <xsl:template match="sky:Field">
<!--        <xsl:apply-templates select="sky:Description" />-->
        <xsl:value-of select="$newline" />
        
        <xsl:text>    public </xsl:text>
        <xsl:apply-templates mode="type" />
        <xsl:text xml:space="preserve"> </xsl:text>
        <xsl:apply-templates select="sky:Name" />
        <xsl:value-of select="$newline" />
        <xsl:text>    {</xsl:text>
        <xsl:apply-templates select="." mode="get" />
        <xsl:apply-templates select="." mode="set" />
        <xsl:value-of select="$newline" />
        <xsl:text>    }</xsl:text>
        <xsl:value-of select="$newline" />
    </xsl:template>
    
    <xsl:template name="SlicedSpan">
        <xsl:param name="index" as="xs:int" />
        <xsl:choose>
            <xsl:when test="$index = 0">
                <xsl:text>_data.Span</xsl:text>
            </xsl:when>
            <xsl:otherwise>
                <xsl:text>_data.Span[</xsl:text>
                <xsl:value-of select="$index" />
                <xsl:text>..]</xsl:text>
            </xsl:otherwise>
        </xsl:choose>
    </xsl:template>
    
    <!-- Getter/Setter Implementations -->
    <xsl:template match="sky:Field[sky:Type = 'Boolean']" mode="get">
        <xsl:param name="index" as="xs:int" />
        <xsl:value-of select="$newline" />
        <xsl:text>        get => </xsl:text>
        <xsl:call-template name="SlicedSpan">
            <xsl:with-param name="index" select="$index" />
        </xsl:call-template>
        <xsl:text>.GetBoolean();</xsl:text>
    </xsl:template>

    <xsl:template match="sky:Field[sky:Type = 'Boolean']" mode="set">
        <xsl:value-of select="$newline" />
        <xsl:text>        set => </xsl:text>
        <xsl:call-template name="SlicedSpan" />
        <xsl:text>.SetBoolean();</xsl:text>
    </xsl:template>

    <xsl:template match="sky:Field[sky:Type = 'Byte']" mode="get">
        <xsl:value-of select="$newline" />
        <xsl:text>        get => </xsl:text>
        <xsl:call-template name="SlicedSpan" />
        <xsl:text>.GetBoolean();</xsl:text>
    </xsl:template>
</xsl:stylesheet>