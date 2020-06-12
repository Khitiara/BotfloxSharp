package khitiara.ffxiv.botflox

import org.springframework.boot.autoconfigure.SpringBootApplication
import org.springframework.boot.context.properties.EnableConfigurationProperties
import org.springframework.boot.runApplication
import org.springframework.cache.annotation.EnableCaching
import org.springframework.context.annotation.Bean
import org.springframework.scheduling.annotation.EnableAsync
import org.springframework.web.reactive.function.client.WebClient


@SpringBootApplication
@EnableCaching
@EnableAsync
@EnableConfigurationProperties
class GearManagementApplication {

    @Bean
    fun webClient(): WebClient =
        WebClient.create("https://xivapi.com")
}

fun main(args: Array<String>) {
    runApplication<GearManagementApplication>(*args)
}
