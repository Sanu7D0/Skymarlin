<?xml version="1.0" encoding="utf-8" ?>
<xsl:stylesheet version="3.0" 
                xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns:sky="urn:Skymarlin">
    <xsl:param name="resultFileName" />
    <xsl:param name="namespace" />
    <xsl:output method="text" indent="yes" />
    <xsl:include href="Common.xslt" />
    
    <xsl:template match="sky:PacketDefinitions">
        <xsl:text>#nullable enable
using System;
using System.Threading;
using Skymarlin.Network;

namespace </xsl:text>
        <xsl:if test="$namespace">
            <xsl:value-of select="$namespace" />
        </xsl:if>
        <xsl:text>;</xsl:text>
        <xsl:text>
public static class ConnectionExtensions
{</xsl:text>
        <xsl:apply-templates select="sky:Packets/sky:Packet" />
        <xsl:text>}</xsl:text>
    </xsl:template>
    
    <xsl:template match="">
        <xsl:value-of select="$newline" />
        <xsl:text>
    public static async ValueTask Send</xsl:text>
        <xsl:apply-templates select="sky:Name" />
        <xsl:text>Async(this IConnection? connection</xsl:text>
        <xsl:if test="count(sky:Fields/sky:Field) > 0">
            <xsl:text>, </xsl:text>
        </xsl:if>
        <xsl:apply-templates select="sky:Fields/sky:Field" mode="params">
            <xsl:sort select="sky:DefaultValue" />
        </xsl:apply-templates>
        <xsl:text>)</xsl:text>
        <xsl:value-of select="$newline" />
        <xsl:text>    {
        if (connection is null)
            return;
        
        int WritePacket()
        {
            var length = </xsl:text>
        <xsl:choose>
            <xsl:when test="sky:Length">
                <xsl:apply-templates select="sky:Name" />
                <xsl:text>Ref.Length</xsl:text>
            </xsl:when>
            <xsl:when test="not(sky:Length)">
                <xsl:apply-templates select="sky:Name" />
                <xsl:text>Ref.Length</xsl:text>
                <xsl:apply-templates select="sky:Fields/sky:Field[(sky:Type = 'String') and not(sky:Length)]" mode="length"/>
                <xsl:text>)</xsl:text>
            </xsl:when>
            <xsl:otherwise>
            throw new NotImplementedException()
            </xsl:otherwise>
        </xsl:choose>
        <xsl:text>;
            var packet = new </xsl:text>
        <xsl:apply-templates select="sky:Name" />
        <xsl:text>Ref.(connection.Output.GetSpan(length)[..length]);</xsl:text>
        <xsl:if test="sky:Fields/sky:Field">
            <xsl:value-of select="$newline" />
            <xsl:apply-templates select="sky:Fields/sky:Field" mode="assignment" />
        </xsl:if>
        <xsl:text>
            return packet.Header.Length;
        }
        
        await connection.SendAsync(WritePacket).ConfigureWait(false);
    }</xsl:text>
    </xsl:template>
</xsl:stylesheet>