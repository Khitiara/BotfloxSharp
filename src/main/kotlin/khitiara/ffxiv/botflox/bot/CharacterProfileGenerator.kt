package khitiara.ffxiv.botflox.bot

import khitiara.ffxiv.botflox.xivapi.CharacterInfo
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.withContext
import org.springframework.stereotype.Service
import org.springframework.web.reactive.function.client.WebClient
import org.springframework.web.reactive.function.client.awaitBody
import java.awt.Color
import java.awt.Font
import java.awt.FontMetrics
import java.awt.Graphics2D
import java.awt.image.BufferedImage
import java.io.ByteArrayInputStream
import java.io.File
import javax.imageio.ImageIO

private const val CORNER_X = 663
private const val CORNER_Y = 589
private const val SPACING_X = 48
private const val SPACING_Y = 83
private const val EUREKA_X = CORNER_X + SPACING_X * 12
private const val EUREKA_Y = CORNER_Y

private val jobIdToXY: Map<Int, IntArray> = mapOf(
    33 to intArrayOf(CORNER_X, CORNER_Y),
    24 to intArrayOf(CORNER_X + SPACING_X,  CORNER_Y),
    28 to intArrayOf(CORNER_X + SPACING_X * 2,  CORNER_Y),
    37 to intArrayOf(CORNER_X + SPACING_X * 4,  CORNER_Y),
    32 to intArrayOf(CORNER_X + SPACING_X * 5,  CORNER_Y),
    21 to intArrayOf(CORNER_X + SPACING_X * 6,  CORNER_Y),
    19 to intArrayOf(CORNER_X + SPACING_X * 7,  CORNER_Y),
    36 to intArrayOf(CORNER_X + SPACING_X * 11, CORNER_Y),

    35 to intArrayOf(CORNER_X, CORNER_Y + SPACING_Y),
    25 to intArrayOf(CORNER_X + SPACING_X, CORNER_Y + SPACING_Y),
    27 to intArrayOf(CORNER_X + SPACING_X * 2, CORNER_Y + SPACING_Y),
    23 to intArrayOf(CORNER_X + SPACING_X * 4, CORNER_Y + SPACING_Y),
    31 to intArrayOf(CORNER_X + SPACING_X * 5, CORNER_Y + SPACING_Y),
    38 to intArrayOf(CORNER_X + SPACING_X * 6, CORNER_Y + SPACING_Y),
    34 to intArrayOf(CORNER_X + SPACING_X * 8, CORNER_Y + SPACING_Y),
    30 to intArrayOf(CORNER_X + SPACING_X * 9, CORNER_Y + SPACING_Y),
    22 to intArrayOf(CORNER_X + SPACING_X * 10, CORNER_Y + SPACING_Y),
    20 to intArrayOf(CORNER_X + SPACING_X * 11, CORNER_Y + SPACING_Y),

    14 to intArrayOf(CORNER_X, CORNER_Y + SPACING_Y * 2),
    15 to intArrayOf(CORNER_X + SPACING_X, CORNER_Y + SPACING_Y * 2),
    13 to intArrayOf(CORNER_X + SPACING_X * 2, CORNER_Y + SPACING_Y * 2),
    12 to intArrayOf(CORNER_X + SPACING_X * 3, CORNER_Y + SPACING_Y * 2),
    8  to intArrayOf(CORNER_X + SPACING_X * 4, CORNER_Y + SPACING_Y * 2),
    11 to intArrayOf(CORNER_X + SPACING_X * 5, CORNER_Y + SPACING_Y * 2),
    10 to intArrayOf(CORNER_X + SPACING_X * 6, CORNER_Y + SPACING_Y * 2),
    9  to intArrayOf(CORNER_X + SPACING_X * 7, CORNER_Y + SPACING_Y * 2),
    17 to intArrayOf(CORNER_X + SPACING_X * 9, CORNER_Y + SPACING_Y * 2),
    16 to intArrayOf(CORNER_X + SPACING_X * 10, CORNER_Y + SPACING_Y * 2),
    18 to intArrayOf(CORNER_X + SPACING_X * 11, CORNER_Y + SPACING_Y * 2)
)

@Service
class CharacterProfileGenerator(private val client: WebClient) {
    suspend fun generateProfileImage(ch: CharacterInfo): BufferedImage {
        val nameFont = Font("SansSerif", Font.PLAIN, 48)
        val titleFont = Font("SansSerif", Font.PLAIN, 30)

        val image = BufferedImage(1310, 873, BufferedImage.TYPE_INT_ARGB)
        val portraitBytes: ByteArray = client.get().uri(ch.portraitUri).retrieve().awaitBody()
        val bg: BufferedImage = withContext(Dispatchers.IO) {
            ImageIO.read(File("src/main/resources/static/characterProfileBackground.png"))
        }
        val portrait: BufferedImage = withContext(Dispatchers.IO) {
            ByteArrayInputStream(portraitBytes).use { ImageIO.read(it) }
        }

        (image.graphics as Graphics2D).run {
            fun drawStringCentered(str: String, metrics: FontMetrics, x: Int, y: Int) {
                drawString(str, x - metrics.stringWidth(str) / 2, y)
            }

            val nameMetrics: FontMetrics = getFontMetrics(nameFont)
            val titleMetrics: FontMetrics = getFontMetrics(titleFont)

            background = Color.WHITE
            clearRect(0, 0, 1310, 873)

            drawImage(bg, 640, 0, null)
            drawImage(portrait, 0, 0, null)

            setFont(nameFont)
            drawStringCentered(ch.name, nameMetrics, 974, if (ch.titleTop == true) 117 else 81)
            drawStringCentered(ch.server + " [" + ch.dc + "]", nameMetrics, 974, 224)

            setFont(titleFont)
            drawStringCentered("<" + ch.title + ">", titleMetrics, 974, if (ch.titleTop == true) 67 else 124)
            drawString(ch.race + ", " + ch.tribe, 688, 327)
            drawString(ch.guardian, 688, 390)

            val gc: String = if (ch.gcRank != null) ch.gcRank else "None"
            drawString(gc, 688, 457)
            val fc: String = if (ch.fcName != null) ch.fcName else "None"
            drawString(fc, 688, 520)

            for ((j, v) in jobIdToXY) {
	            drawStringCentered("a", titleMetrics, v[0] + 21, v[1] + 60)
            }
        }
        return image
    }
}